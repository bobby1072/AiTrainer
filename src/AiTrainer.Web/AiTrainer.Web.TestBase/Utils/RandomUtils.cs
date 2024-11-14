namespace AiTrainer.Web.TestBase.Utils
{
    public static class RandomUtils
    {
        public static DateTime DateInThePast(DateTime earliestDate)
        {
            if (DateTime.UtcNow <= earliestDate)
            {
                throw new InvalidOperationException("Cannot get dates after date given in past");
            }
            Random gen = new Random();
            int range = (DateTime.UtcNow.AddDays(-3) - earliestDate).Days;

            return earliestDate.AddDays(gen.Next(range));

        }
        public static DateTime DateInThePast()
        {
            var earliestDate = new DateTime(1999, 1, 1);
            return DateInThePast(earliestDate);
        }
        public static DateTime DateInTheFuture()
        {
            return DateInThePast(DateTime.UtcNow.AddYears(20));
        }
        public static DateTime DateInTheFuture(DateTime maxDate)
        {
            if (DateTime.UtcNow >= maxDate)
            {
                throw new InvalidOperationException("Cannot get dates after date given in past");
            }
            Random gen = new Random();
            int range = (maxDate.AddDays(-3) - DateTime.UtcNow).Days;

            return maxDate.AddDays(-gen.Next(range));
        }
    }
}
