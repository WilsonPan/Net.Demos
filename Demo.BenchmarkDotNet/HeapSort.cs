namespace Demo.BenchmarkDotNet
{
    public class HeapSort
    {
        public static void Sort(int[] nums)
        {
            //构建最大堆
            BuildMaxHeap(nums);

            for (int i = nums.Length - 1; i > 0; i--)
            {
                Swap(nums, 0, i);           //将堆顶数据放在数组后面
                Heapify(nums, 0, i - 1);    //重新构建len - 1的大顶堆
            }
        }

        //构建大顶堆
        public static void BuildMaxHeap(int[] nums)
        {
            //从非叶子点，底部开始构建，大的数从底部冒到根部
            for (int i = nums.Length / 2 - 1; i >= 0; i--)
            {
                Heapify(nums, i, nums.Length);
            }
        }

        //堆调整
        private static void Heapify(int[] nums, int current, int length)
        {
            //获取左右子节点，将当前节点保存再临时变量
            var left = current * 2 + 1;
            var right = current * 2 + 2;
            var small = current;

            if (left < length && nums[left] > nums[small])
                small = left;

            if (right < length && nums[right] > nums[small])
                small = right;

            if (small != current)   //子节点有比当前节点大的值
            {
                Swap(nums, current, small);
                Heapify(nums, small, length);   //因为字节点发生交换，递归子节点的调整，确保子节点大于等于它的子节点
            }
        }

        private static void Swap(int[] nums, int i, int j)
        {
            var temp = nums[i];
            nums[i] = nums[j];
            nums[j] = temp;
        }
    }
}