using Domain.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Repository.Expressions
{
    public class JsonExpressionParser
    {
        private readonly string StringStr = "string";
        private readonly string BooleanStr = nameof(Boolean).ToLower();
        private readonly string Number = nameof(Number).ToLower();
        private readonly string In = nameof(In).ToLower();
        private readonly string And = nameof(And).ToLower();

        private readonly MethodInfo MethodListContains = typeof(Enumerable).GetMethods(
                        BindingFlags.Static | BindingFlags.Public)
                        .Single(m => m.Name == nameof(Enumerable.Contains)
                            && m.GetParameters().Length == 2);

        private delegate Expression Binder(Expression left, Expression right);

        public LambdaExpression Parse<T>(JsonDocument doc)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            Expression conditions = ParseTree<T>(doc.RootElement, parameter);
            if (conditions.CanReduce)
            {
                conditions = conditions.ReduceAndCheck();
            }
            LambdaExpression result = Expression.Lambda<Func<T, bool>>(conditions, parameter);
            return result;
        }

        private Expression ParseTree<T>(JsonElement condition, ParameterExpression parm)
        {
            Expression left = null;
            var gate = condition.GetProperty(nameof(condition)).GetString();
            
            JsonElement rules = condition.GetProperty(nameof(rules));

            Binder binder = gate == And ? Expression.And : Expression.Or;

            Expression bind(Expression left, Expression right) =>
                left == null ? right : binder(left, right);

            foreach (var rule in rules.EnumerateArray())
            {
                if (rule.TryGetProperty(nameof(condition), out JsonElement check))
                {
                    var rightCheck = ParseTree<T>(rule, parm);
                    left = bind(left, rightCheck);
                    continue;
                }
                
                string @operator = rule.GetProperty(nameof(@operator)).GetString();
                string type = rule.GetProperty(nameof(type)).GetString();
                string field = rule.GetProperty(nameof(field)).GetString();
                
                JsonElement value = rule.GetProperty(nameof(value));
                MemberExpression property = Expression.Property(parm, field);
                var rightOperator = ParseOperator(@operator, type, value, property);
                left = bind(left, rightOperator);
            }

            return left;
        }

        private Expression ParseOperator(string @operator, string type, JsonElement value, MemberExpression property)
        {
            OperacaoFiltro operacao = OperacaoFiltroExtensions.FromString(@operator);
            Expression result = null;
            switch (operacao)
            {
                case OperacaoFiltro.Igual:
                    result = ParseOperatorEqual(type, value, property);
                    break;
                case OperacaoFiltro.Diferente:
                    result = ParseOperatorNotEqual(type, value, property);
                    break;
                case OperacaoFiltro.Contem:
                    result = ParseOperatorContains(type, value, property);
                    break;
                case OperacaoFiltro.NaoContem:
                    result = ParseOperatorNotContains(type, value, property);
                    break;
                case OperacaoFiltro.MaiorOuIgual:
                    result = ParseOperatorGreaterThanOrEqual(type, value, property);
                    break;
                case OperacaoFiltro.Maior:
                    result = ParseOperatorGreaterThan(type, value, property);
                    break;
                case OperacaoFiltro.MenorOuIgual:
                    result = ParseOperatorLessThanOrEqual(type, value, property);
                    break;
                case OperacaoFiltro.Menor:
                    result = ParseOperatorLessThan(type, value, property);
                    break;
                case OperacaoFiltro.ListaContem:
                    result = ParseOperatorIn(type, value, property);
                    break;
                case OperacaoFiltro.ListaNaoContem:
                    result = ParseOperatorNotIn(type, value, property);
                    break;
            }
            return result;
        }

        private ConstantExpression ParseConstant(string type, JsonElement value)
        {
            object val = (type == StringStr || type == BooleanStr) ?
                        value.GetString() : value.GetInt32();
            //value.GetString() : value.GetDecimal();
            return Expression.Constant(val);
        }

        private Expression ParseOperatorEqual(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            BinaryExpression result = Expression.Equal(property, constant);
            return result;
        }

        private Expression ParseOperatorNotEqual(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            BinaryExpression result = Expression.NotEqual(property, constant);
            return result;
        }

        private Expression ParseOperatorContains(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            MethodCallExpression result = Expression.Call(property, "Contains", Type.EmptyTypes, constant);
            return result;
        }

        private Expression ParseOperatorNotContains(string type, JsonElement value, MemberExpression property)
        {
            return Expression.Not(ParseOperatorContains(type, value, property));
        }

        private Expression ParseOperatorGreaterThanOrEqual(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            BinaryExpression result = Expression.GreaterThanOrEqual(property, constant);
            return result;
        }

        private Expression ParseOperatorGreaterThan(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            BinaryExpression result = Expression.GreaterThan(property, constant);
            return result;
        }

        private Expression ParseOperatorLessThanOrEqual(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            BinaryExpression result = Expression.LessThanOrEqual(property, constant);
            return result;
        }

        private Expression ParseOperatorLessThan(string type, JsonElement value, MemberExpression property)
        {
            ConstantExpression constant = ParseConstant(type, value);
            BinaryExpression result = Expression.LessThan(property, constant);
            return result;
        }

        private Expression ParseOperatorIn(string type, JsonElement value, MemberExpression property)
        {
            MethodInfo method = MethodListContains.MakeGenericMethod(typeof(string));
            List<string?> listVal = value.EnumerateArray().Select(e => e.GetString()).ToList();
            ConstantExpression constant = Expression.Constant(listVal);
            MethodCallExpression result = Expression.Call(method, property, constant);
            return result;
        }

        private Expression ParseOperatorNotIn(string type, JsonElement value, MemberExpression property)
        {
            return Expression.Not(ParseOperatorIn(type, value, property));
        }
    }
}