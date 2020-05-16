namespace Demo.BenchmarkDotNet
{
    public class QuickSort
    {
        public static void Sort(int[] nums) => Sort(nums, 0, nums.Length - 1);

        public static void Sort(int[] nums, int left, int right)
        {
            //1. 赋值到临时变量，需要保留原始值递归
            var i = left;
            var j = right;
            var pivot = nums[i];     //2. 选数组第一个作为基准，挖第一个坑（nums[i]）
            while (i < j)
            {
                //3. 从右边出发，寻找小于pivot的值
                while (i < j && nums[j] > pivot) j--;
                nums[i] = nums[j];      //补上nums[i]的坑，挖了nums[j]的坑

                //4. 从左边出发，寻找大于pivot的值
                while (i < j && nums[i] <= pivot) i++;
                nums[j] = nums[i];      //补上nums[j]的坑，挖了nums[i]的坑
            }
            nums[i] = pivot; //基准补上最后挖的坑
                             //到这，i == j，nums[i] 的左边都小于等于pivot，num[i] 右边大于pivot

            if (left < (i - 1)) Sort(nums, left, i - 1);       //递归左边数组，判断为了减少一次递归
            if (right > (i + 1)) Sort(nums, i + 1, right);     //递归右边数组，判断为了减少一次递归
        }
    }
}