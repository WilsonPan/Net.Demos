namespace Demo.BenchmarkDotNet
{
    public class BubbleSort
    {
        public static void Sort(int[] datas)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                for (int j = i + 1; j < datas.Length; j++)
                {
                    if (datas[i] > datas[j])
                    {
                        var temp = datas[i];
                        datas[i] = datas[j];
                        datas[j] = temp;
                    }
                }
            }
        }
    }
}