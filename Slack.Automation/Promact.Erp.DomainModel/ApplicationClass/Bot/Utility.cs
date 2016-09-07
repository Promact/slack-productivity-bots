using System;
using System.Collections.Generic;


namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{
    public class Utility
    {
        public static dynamic TryGetProperty(dynamic dynamicObject, String PropertyName)
        {
            return TryGetProperty(dynamicObject, PropertyName, "");
        }

        //public static dynamic TryGetProperty(dynamic dynamicObject, String PropertyName, dynamic Default)
        //{
        //    try
        //    {
        //        if (!HasProperty(dynamicObject, PropertyName))
        //        {
        //            return Default;
        //        }
        //        //if (dynamicObject.GetType() == typeof(System.Web.Helpers.DynamicJsonObject))
        //        //{
        //        // good thing this type of documentation was easy to find


        //        System.Web.Helpers.DynamicJsonObject obj = (System.Web.Helpers.DynamicJsonObject)dynamicObject;



        //        Type scope = obj.GetType();
        //        System.Dynamic.IDynamicMetaObjectProvider provider = obj as System.Dynamic.IDynamicMetaObjectProvider;
        //        if (provider != null)
        //        {
        //            System.Linq.Expressions.ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(object));
        //            System.Dynamic.DynamicMetaObject mobj = provider.GetMetaObject(param);
        //            System.Dynamic.GetMemberBinder binder = (System.Dynamic.GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, PropertyName, scope, new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) });
        //            System.Dynamic.DynamicMetaObject ret = mobj.BindGetMember(binder);
        //            System.Linq.Expressions.BlockExpression final = System.Linq.Expressions.Expression.Block(
        //                System.Linq.Expressions.Expression.Label(System.Runtime.CompilerServices.CallSiteBinder.UpdateLabel),
        //                ret.Expression
        //            );
        //            System.Linq.Expressions.LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(final, param);
        //            Delegate del = lambda.Compile();
        //            return del.DynamicInvoke(obj);
        //        }
        //        else
        //        {
        //            return obj.GetType().GetProperty(PropertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(obj, null);
        //        }
        //        //}
        //        //else if (dynamicObject.GetType() == typeof(System.Collections.IDictionary))
        //        //{
        //        //    return (Dictionary<String, object>)dynamicObject[PropertyName];
        //        //}
        //        return Default;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Could not determine if dynamic object has property.", ex);
        //    }
        //}


        public static dynamic TryGetProperty(dynamic dynamicObject, String PropertyName, dynamic Default)
        {
            try
            {
                dynamic obj = dynamicObject;

                Type scope = obj.GetType();
                System.Dynamic.IDynamicMetaObjectProvider provider = obj as System.Dynamic.IDynamicMetaObjectProvider;
                if (provider != null)
                {
                    System.Linq.Expressions.ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    System.Dynamic.DynamicMetaObject mobj = provider.GetMetaObject(param);
                    System.Dynamic.GetMemberBinder binder = (System.Dynamic.GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, PropertyName, scope, new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) });
                    System.Dynamic.DynamicMetaObject ret = mobj.BindGetMember(binder);
                    System.Linq.Expressions.BlockExpression final = System.Linq.Expressions.Expression.Block(
                        System.Linq.Expressions.Expression.Label(System.Runtime.CompilerServices.CallSiteBinder.UpdateLabel),
                        ret.Expression
                    );
                    System.Linq.Expressions.LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(final, param);
                    Delegate del = lambda.Compile();
                    return del.DynamicInvoke(obj);
                }
                else
                {
                    var getype = obj.GetType();
                    if (getype != null)
                    {
                        var prop = getype.GetProperty(PropertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (prop != null)
                        {
                            var value = prop.GetValue(obj, null);
                            return value;
                        }
                    }
                    else
                        return Default;
                    //return obj.GetType().GetProperty(PropertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(obj, null);
                }
                return Default;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not determine if dynamic object has property.", ex);
            }
        }


        //public static dynamic HasProperty(dynamic dynamicObject, String PropertyName)
        //{
        //    try
        //    {
        //        if (dynamicObject.GetType() == typeof(System.Web.Helpers.DynamicJsonObject))
        //        {
        //            System.Web.Helpers.DynamicJsonObject obj = (System.Web.Helpers.DynamicJsonObject)dynamicObject;
        //            foreach (String strName in obj.GetDynamicMemberNames())
        //            {
        //                if (strName == PropertyName)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        else if (dynamicObject.GetType() == typeof(System.Collections.IDictionary))
        //        {
        //            if (((IDictionary<String, object>)dynamicObject).ContainsKey(PropertyName))
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Could not determine if dynamic object has property.", ex);
        //    }
        //}



        public static dynamic HasProperty(dynamic dynamicObject, String PropertyName)
        {
            try
            {
                //    if (dynamicObject.GetType() == typeof(System.Web.Helpers.DynamicJsonObject))
                //    {
                //       System.Web.Helpers.DynamicJsonObject obj = (System.Web.Helpers.DynamicJsonObject)dynamicObject;
                //        foreach (String strName in obj.GetDynamicMemberNames())
                //        {
                //            if (strName == PropertyName)
                //            {
                //                return true;
                //            }
                //        }
                //    }
                //Newtonsoft.Json.JsonConvert.DeserializeObject(jsonstring);
                //    if (dynamicObject.GetType() == typeof(System.Web.Helpers.DynamicJsonObject))
                // if (1==1)
                //  {
                if (dynamicObject.GetType() == typeof(Newtonsoft.Json.JsonObjectAttribute))
                {
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(dynamicObject);
                    // foreach(var strng in df.)

                    //  }
                    //   System.Web.Helpers.DynamicJsonObject obj = (System.Web.Helpers.DynamicJsonObject)dynamicObject;
                    ////var obj = dynamicObject;
                    //foreach (String strName in obj.GetDynamicMemberNames())
                    //{
                    //    if (strName == PropertyName)
                    //    {
                    //        return true;
                    //    }
                    //}
                }

                else if (dynamicObject.GetType() == typeof(System.Collections.IDictionary))
                {
                    if (((IDictionary<String, object>)dynamicObject).ContainsKey(PropertyName))
                    {
                        return true;
                    }
                }
                else
                {
                    // JsonConvert.DeserializeObject<SlackOAuthResponse>(responseContent);
                    var obj = new Object();
                    //switch (PropertyName)
                    //{
                    //    case "bots":
                    //        obj = new BotDetails();
                    //        break;
                    //    default:
                    //        obj = (BotDetails)obj;
                    //        break;
                    //}

                    //dynamic obj1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(dynamicObject);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not determine if dynamic object has property.", ex);
            }
        }
    }
}
