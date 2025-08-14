namespace OtoBatchEditor
{
    public static class NumValidation
    {
        public static int IntValidation(string input, int defaultValue, int min, int max, out bool valid)
        {
            if (int.TryParse(input, out int result) && result >= min && result <= max)
            {
                valid = true;
                return result;
            }
            else
            {
                valid = false;
                return defaultValue;
            }
        }

        public static double DoubleValidation(string input, double defaultValue, double min, double max, out bool valid)
        {
            if (double.TryParse(input, out double result) && result >= min && result <= max)
            {
                valid = true;
                return result;
            }
            else
            {
                valid = false;
                return defaultValue;
            }
        }
    }
}
