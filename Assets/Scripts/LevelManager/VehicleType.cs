namespace LevelManager
{
    /*
     * This enum is used to defined types of vehicle and used as filename for dynamic loading.
     */
    public enum VehicleType
    {
        black,
        blue,
        green,
        red,
        silver,
        taxi,
        police,
        ambulance,
        garbage,
        garbage_a,
        garbage_b,
        garbage_c,
        garbage_d,
        garbage_e,
        empty,
        random
    }
    
    public static class Extensions
    {        
        public static string ToCapitalizedString(this VehicleType type)
        {
            return char.ToUpper(type.ToString()[0]) + type.ToString().Substring(1);
        }
    }
}