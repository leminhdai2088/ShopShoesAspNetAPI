namespace ShopShoesAPI.common
{
    public static class StaticMethod
    {
        public static object GetValObjDy(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }
    }
}
