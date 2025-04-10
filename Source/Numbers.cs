namespace WPlugZ_CLI
{

    public static class Numbers
    {

        /// <summary>
        /// Clamps an integer.
        /// </summary>
        /// <param name="integer">Integer to clamp</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns>The clamped integer</returns>
        public static int ClampInteger(int integer, int min, int max)
        {

            if (integer < min)
            {
                return min;
            }

            return integer > max ? max : integer;

        }

    }

}