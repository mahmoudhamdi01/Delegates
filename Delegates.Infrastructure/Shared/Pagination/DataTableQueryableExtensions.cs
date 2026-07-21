using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Shared.Pagination
{
    public static class DataTableQueryableExtensions
    {
        public static IQueryable<T> ApplyDataTableFiltering<T>(
            this IQueryable<T> query,
            DataTablePaginationRequestDto request,
            IDictionary<string, string>? columnMap = null,
            bool allowGlobalSearch = true)
        {
            // 1) فلاتر أعمدة بقيمة واحدة
            if (request.SearchableCloumnsValues is { Count: > 0 })
            {
                foreach (var kv in request.SearchableCloumnsValues)
                {
                    var val = kv.Value?.Trim();
                    if (string.IsNullOrWhiteSpace(val)) continue;

                    var col = MapColumn(kv.Key, columnMap);
                    if (string.IsNullOrWhiteSpace(col)) continue;

                    query = query.Where(BuildPredicate<T>(col, val));
                }
            }

            // 2) فلاتر أعمدة بقيم متعددة (IN) - مثال: Status = [1,2] أو BranchId = ["3","5"]
            if (request.SearchableCloumnsMultiValues is { Count: > 0 })
            {
                foreach (var kv in request.SearchableCloumnsMultiValues)
                {
                    if (kv.Value is null || kv.Value.Length == 0) continue;

                    var col = MapColumn(kv.Key, columnMap);
                    if (string.IsNullOrWhiteSpace(col)) continue;

                    var param = Expression.Parameter(typeof(T), "x");
                    var expr = BuildMultiValueExpression(param, col!, kv.Value);
                    if (expr is null) continue;

                    query = query.Where(Expression.Lambda<Func<T, bool>>(expr, param));
                }
            }

            // 3) فلاتر نطاق تاريخ
            if (request.DateRangeFilters is { Count: > 0 })
            {
                foreach (var kv in request.DateRangeFilters)
                {
                    if (kv.Value is null || (kv.Value.From is null && kv.Value.To is null)) continue;

                    var col = MapColumn(kv.Key, columnMap);
                    if (string.IsNullOrWhiteSpace(col)) continue;

                    var param = Expression.Parameter(typeof(T), "x");
                    var expr = BuildDateRangeExpression(param, col!, kv.Value.From, kv.Value.To);
                    if (expr is null) continue;

                    query = query.Where(Expression.Lambda<Func<T, bool>>(expr, param));
                }
            }

            // 4) البحث العام (Global Search)
            if (allowGlobalSearch)
            {
                var search = request.SearchValue?.Trim();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var cols = request.SearchableCloumns?
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .ToArray();

                    // لو مفيش أعمدة محددة، فتّش في كل خصائص الـ string
                    if (cols is null || cols.Length == 0)
                    {
                        cols = typeof(T).GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.Name)
                            .ToArray();
                    }

                    var mappedCols = cols
                        .Select(c => MapColumn(c, columnMap))
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToArray();

                    if (mappedCols.Length > 0)
                        query = query.Where(BuildGlobalSearchPredicate<T>(mappedCols!, search));
                }
            }

            return query;
        }

        public static IQueryable<T> ApplyDataTableSorting<T>(
            this IQueryable<T> query,
            DataTablePaginationRequestDto request,
            IDictionary<string, string>? columnMap = null,
            string defaultSortColumn = "Id")
        {
            var col = request.SortColumnName?.Trim();
            col = string.IsNullOrWhiteSpace(col) ? defaultSortColumn : col;

            if (string.IsNullOrWhiteSpace(col)) return query;

            col = MapColumn(col, columnMap);
            var desc = string.Equals(request.SortColumnDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return query.OrderByDynamic(col!, desc);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string columnPath, bool desc)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var member = BuildMemberAccess(param, columnPath);
            if (member is null) return query;

            var keySelector = Expression.Lambda(member, param);
            var methodName = desc ? "OrderByDescending" : "OrderBy";

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), member.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, keySelector })!;
        }

        private static Expression<Func<T, bool>> BuildGlobalSearchPredicate<T>(string[] columns, string searchValue)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression? body = null;

            foreach (var col in columns)
            {
                var expr = BuildEqualsOrContainsExpression(param, col, searchValue);
                if (expr is null) continue;

                body = body is null ? expr : Expression.OrElse(body, expr);
            }

            body ??= Expression.Constant(true);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression<Func<T, bool>> BuildPredicate<T>(string columnPath, string value)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var expr = BuildEqualsOrContainsExpression(param, columnPath, value) ?? Expression.Constant(true);
            return Expression.Lambda<Func<T, bool>>(expr, param);
        }

        private static Expression? BuildEqualsOrContainsExpression(ParameterExpression param, string columnPath, string value)
        {
            var member = BuildMemberAccess(param, columnPath);
            if (member is null) return null;

            var underlying = Nullable.GetUnderlyingType(member.Type);
            var nonNullType = underlying ?? member.Type;
            var normalizedValue = NormalizeNumericText(value);

            // string => Contains
            if (nonNullType == typeof(string))
            {
                var notNull = Expression.NotEqual(member, Expression.Constant(null, member.Type));
                var contains = Expression.Call(
                    Expression.Convert(member, typeof(string)),
                    nameof(string.Contains), Type.EmptyTypes, Expression.Constant(value));

                return Expression.AndAlso(notNull, contains);
            }

            (Expression hasValueGuard, Expression memberValue) = UnwrapNullable(member, underlying);

            // numeric => equals OR contains
            if (IsNumericType(nonNullType))
            {
                var toStringCall = Expression.Call(memberValue, nameof(object.ToString), Type.EmptyTypes);
                var containsExpr = Expression.Call(toStringCall, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(normalizedValue));
                var equalsExpr = TryBuildNumericEquals(memberValue, nonNullType, normalizedValue);

                Expression combined = equalsExpr is null ? containsExpr : Expression.OrElse(equalsExpr, containsExpr);
                return underlying is null ? combined : Expression.AndAlso(hasValueGuard, combined);
            }

            // enum => equals OR contains
            if (nonNullType.IsEnum)
            {
                var toStringCall = Expression.Call(memberValue, nameof(object.ToString), Type.EmptyTypes);
                var containsExpr = Expression.Call(toStringCall, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(value));

                Expression? equalsExpr = null;
                if (Enum.TryParse(nonNullType, value, true, out var enumVal))
                    equalsExpr = Expression.Equal(memberValue, Expression.Constant(enumVal));

                Expression combined = equalsExpr is null ? containsExpr : Expression.OrElse(equalsExpr, containsExpr);
                return underlying is null ? combined : Expression.AndAlso(hasValueGuard, combined);
            }

            // DateTime => نطاق اليوم OR contains
            if (nonNullType == typeof(DateTime))
            {
                Expression? rangeExpr = null;
                if (DateTime.TryParse(value, out var dt))
                {
                    var start = dt.Date;
                    var end = start.AddDays(1);
                    var ge = Expression.GreaterThanOrEqual(memberValue, Expression.Constant(start));
                    var lt = Expression.LessThan(memberValue, Expression.Constant(end));
                    rangeExpr = Expression.AndAlso(ge, lt);
                }

                var toStringCall = Expression.Call(memberValue, nameof(object.ToString), Type.EmptyTypes);
                var containsExpr = Expression.Call(toStringCall, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(value));

                Expression combined = rangeExpr is null ? containsExpr : Expression.OrElse(rangeExpr, containsExpr);
                return underlying is null ? combined : Expression.AndAlso(hasValueGuard, combined);
            }

            return null;
        }

        private static Expression? BuildMultiValueExpression(ParameterExpression param, string columnPath, string[] values)
        {
            var member = BuildMemberAccess(param, columnPath);
            if (member is null) return null;

            var underlying = Nullable.GetUnderlyingType(member.Type);
            var nonNullType = underlying ?? member.Type;

            Expression? body = null;
            foreach (var raw in values)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var eq = BuildSingleEquals(member, nonNullType, underlying, raw.Trim());
                if (eq is null) continue;

                body = body is null ? eq : Expression.OrElse(body, eq);
            }

            return body;
        }

        private static Expression? BuildSingleEquals(MemberExpression member, Type nonNullType, Type? underlying, string raw)
        {
            (Expression hasValueGuard, Expression memberValue) = UnwrapNullable(member, underlying);
            Expression? eq = null;

            if (nonNullType == typeof(string))
            {
                eq = Expression.Equal(memberValue, Expression.Constant(raw));
            }
            else if (nonNullType.IsEnum)
            {
                if (Enum.TryParse(nonNullType, raw, true, out var enumVal))
                    eq = Expression.Equal(memberValue, Expression.Constant(enumVal));
                else if (int.TryParse(raw, out var enumInt) && Enum.IsDefined(nonNullType, enumInt))
                    eq = Expression.Equal(memberValue, Expression.Constant(Enum.ToObject(nonNullType, enumInt)));
            }
            else if (nonNullType == typeof(Guid))
            {
                if (Guid.TryParse(raw, out var g))
                    eq = Expression.Equal(memberValue, Expression.Constant(g));
            }
            else if (nonNullType == typeof(bool))
            {
                if (bool.TryParse(raw, out var b))
                    eq = Expression.Equal(memberValue, Expression.Constant(b));
            }
            else if (IsNumericType(nonNullType))
            {
                eq = TryBuildNumericEquals(memberValue, nonNullType, NormalizeNumericText(raw));
            }

            if (eq is null) return null;
            return underlying is null ? eq : Expression.AndAlso(hasValueGuard, eq);
        }
        private static Expression? BuildDateRangeExpression(ParameterExpression param, string columnPath, DateTime? from, DateTime? to)
        {
            var member = BuildMemberAccess(param, columnPath);
            if (member is null) return null;

            var underlying = Nullable.GetUnderlyingType(member.Type);
            if ((underlying ?? member.Type) != typeof(DateTime)) return null;

            (Expression hasValueGuard, Expression memberValue) = UnwrapNullable(member, underlying);

            Expression? body = null;

            if (from.HasValue)
                body = Expression.GreaterThanOrEqual(memberValue, Expression.Constant(from.Value.Date));

            if (to.HasValue)
            {
                var lt = Expression.LessThan(memberValue, Expression.Constant(to.Value.Date.AddDays(1)));
                body = body is null ? lt : Expression.AndAlso(body, lt);
            }

            if (body is null) return null;
            return underlying is null ? body : Expression.AndAlso(hasValueGuard, body);
        }

        private static (Expression hasValueGuard, Expression memberValue) UnwrapNullable(MemberExpression member, Type? underlying)
        {
            if (underlying is null)
                return (Expression.Constant(true), member);

            return (Expression.Property(member, "HasValue"), Expression.Property(member, "Value"));
        }

        private static string? MapColumn(string column, IDictionary<string, string>? map)
        {
            if (map is null) return column;
            return map.TryGetValue(column, out var mapped) ? mapped : column;
        }

        private static MemberExpression? BuildMemberAccess(Expression param, string path)
        {
            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            Expression? current = param;

            foreach (var p in parts)
            {
                var prop = current!.Type.GetProperty(p);
                if (prop is null) return null;
                current = Expression.Property(current, prop);
            }

            return current as MemberExpression;
        }

        private static bool IsNumericType(Type t) =>
            t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long) ||
            t == typeof(float) || t == typeof(double) || t == typeof(decimal);

        private static string NormalizeNumericText(string input) =>
            input?.Trim()?.Replace(',', '.') ?? string.Empty;

        private static Expression? TryBuildNumericEquals(Expression memberValue, Type nonNullType, string value)
        {
            if (nonNullType == typeof(int))
            {
                if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) || int.TryParse(value, out i))
                    return Expression.Equal(memberValue, Expression.Constant(i));
            }
            else if (nonNullType == typeof(long))
            {
                if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var l) || long.TryParse(value, out l))
                    return Expression.Equal(memberValue, Expression.Constant(l));
            }
            else if (nonNullType == typeof(decimal))
            {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ||
                    decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return Expression.Equal(memberValue, Expression.Constant(d));
            }
            else if (nonNullType == typeof(double))
            {
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var dd) ||
                    double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out dd))
                    return Expression.Equal(memberValue, Expression.Constant(dd));
            }
            else if (nonNullType == typeof(float))
            {
                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ||
                    float.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out f))
                    return Expression.Equal(memberValue, Expression.Constant(f));
            }
            else if (nonNullType == typeof(byte))
            {
                if (byte.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var by) || byte.TryParse(value, out by))
                    return Expression.Equal(memberValue, Expression.Constant(by));
            }
            else if (nonNullType == typeof(short))
            {
                if (short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var sh) || short.TryParse(value, out sh))
                    return Expression.Equal(memberValue, Expression.Constant(sh));
            }

            return null;
        }
    }
}
