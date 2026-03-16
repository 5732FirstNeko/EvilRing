using System;
using System.Collections;
using System.Collections.Generic;

namespace PriorityQueue
{
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly List<(T Item, int Priority)> _heap;
        // 优先级比较器（true=最小优先，false=最大优先）
        private readonly IComparer<int> _comparer;
        private readonly IEqualityComparer<T> _itemComparer;

        /// <summary>
        /// 队列元素数量
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// 是否为空队列
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// 初始化优先队列（默认最小优先 + 默认元素比较器）
        /// </summary>
        public PriorityQueue() : this(Comparer<int>.Default, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// 初始化优先队列（自定义优先级规则 + 默认元素比较器）
        /// </summary>
        /// <param name="isMaxPriority">是否最大优先（true=优先级值越大越先出队）</param>
        public PriorityQueue(bool isMaxPriority)
            : this(
                isMaxPriority ? Comparer<int>.Create((a, b) => b.CompareTo(a)) : Comparer<int>.Default,
                EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// 初始化优先队列（自定义优先级比较器 + 默认元素比较器）
        /// </summary>
        /// <param name="comparer">优先级比较器</param>
        public PriorityQueue(IComparer<int> comparer)
            : this(comparer, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// 初始化优先队列（自定义优先级比较器 + 自定义元素比较器）
        /// </summary>
        /// <param name="priorityComparer">优先级比较器</param>
        /// <param name="itemComparer">元素相等性比较器</param>
        public PriorityQueue(IComparer<int> priorityComparer, IEqualityComparer<T> itemComparer)
        {
            _comparer = priorityComparer ?? Comparer<int>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<T>.Default;
            _heap = new List<(T, int)>();
        }

        /// <summary>
        /// 入队：添加元素并按优先级调整堆结构
        /// </summary>
        /// <param name="item">要入队的元素</param>
        /// <param name="priority">优先级（默认最小优先：值越小越先出队）</param>
        public void Enqueue(T item, int priority)
        {
            // 1. 将新元素添加到堆末尾
            _heap.Add((item, priority));
            // 2. 向上调整堆（堆化），保证堆的性质
            HeapifyUp(_heap.Count - 1);
        }

        /// <summary>
        /// 出队：取出并移除优先级最高的元素
        /// </summary>
        /// <returns>优先级最高的元素</returns>
        /// <exception cref="InvalidOperationException">队列为空时抛出</exception>
        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("优先队列为空，无法执行出队操作");

            // 1. 获取堆顶元素（优先级最高）
            var topItem = _heap[0];
            // 2. 将最后一个元素移到堆顶
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            // 3. 向下调整堆，保证堆的性质
            if (!IsEmpty)
                HeapifyDown(0);

            return topItem.Item;
        }

        /// <summary>
        /// 查看队首元素（优先级最高），不移除
        /// </summary>
        /// <returns>优先级最高的元素</returns>
        /// <exception cref="InvalidOperationException">队列为空时抛出</exception>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("优先队列为空，无法查看队首元素");

            return _heap[0].Item;
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>
        /// 修改指定元素的优先级
        /// </summary>
        /// <param name="item">要修改优先级的元素</param>
        /// <param name="newPriority">新的优先级值</param>
        /// <returns>是否修改成功（元素不存在则返回false）</returns>
        /// <exception cref="InvalidOperationException">队列为空时抛出</exception>
        public bool UpdatePriority(T item, int newPriority)
        {
            if (IsEmpty)
                throw new InvalidOperationException("优先队列为空，无法修改优先级");

            // 1. 找到元素在堆中的索引
            int itemIndex = -1;
            for (int i = 0; i < _heap.Count; i++)
            {
                if (_itemComparer.Equals(_heap[i].Item, item))
                {
                    itemIndex = i;
                    break;
                }
            }

            // 元素不存在，返回false
            if (itemIndex == -1)
                return false;

            // 2. 记录旧优先级，修改为新优先级
            int oldPriority = _heap[itemIndex].Priority;
            _heap[itemIndex] = (item, newPriority);

            // 3. 根据新旧优先级的大小，调整堆结构
            // 比较逻辑：新优先级是否比旧优先级更"优"（更小/更大，取决于比较器）
            int compareResult = _comparer.Compare(newPriority, oldPriority);
            if (compareResult < 0)
            {
                // 新优先级更优 → 向上堆化（元素需要靠近堆顶）
                HeapifyUp(itemIndex);
            }
            else if (compareResult > 0)
            {
                // 新优先级更差 → 向下堆化（元素需要远离堆顶）
                HeapifyDown(itemIndex);
            }
            // 优先级未变，无需调整

            return true;
        }

        /// <summary>
        /// 移除队列中指定的元素，并重新堆化保持优先队列性质
        /// </summary>
        /// <param name="item">要移除的元素</param>
        /// <returns>移除成功返回true，元素不存在返回false</returns>
        /// <exception cref="InvalidOperationException">队列为空时抛出</exception>
        public bool Remove(T item)
        {
            // 空队列校验
            if (IsEmpty)
                throw new InvalidOperationException("优先队列为空，无法移除元素");

            // 1. 查找元素在堆中的索引
            int removeIndex = -1;
            for (int i = 0; i < _heap.Count; i++)
            {
                if (_itemComparer.Equals(_heap[i].Item, item))
                {
                    removeIndex = i;
                    break;
                }
            }

            // 元素不存在，返回false
            if (removeIndex == -1)
                return false;

            // 2. 处理移除逻辑：将最后一个元素移到移除位置，再移除最后一个元素
            int lastIndex = _heap.Count - 1;
            // 如果移除的是最后一个元素，直接移除无需堆化
            if (removeIndex == lastIndex)
            {
                _heap.RemoveAt(lastIndex);
                return true;
            }

            // 3. 非最后一个元素：将最后一个元素替换到移除位置
            _heap[removeIndex] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);

            // 4. 重新堆化：先尝试向上堆化，若未调整则向下堆化
            // （因为替换后的元素可能比父节点更优，或比子节点更差）
            int originalIndex = removeIndex;
            HeapifyUp(removeIndex);

            // 如果向上堆化后索引未变，说明需要向下堆化
            if (removeIndex == originalIndex)
            {
                HeapifyDown(removeIndex);
            }

            return true;
        }

        #region 堆调整核心方法
        /// <summary>
        /// 向上堆化（新增元素时，从下往上调整）
        /// </summary>
        /// <param name="index">当前元素索引</param>
        private void HeapifyUp(int index)
        {
            // 根节点（索引0）无需调整
            while (index > 0)
            {
                // 父节点索引：(i-1)/2
                int parentIndex = (index - 1) / 2;
                // 比较当前元素和父节点的优先级：如果当前元素优先级更高，交换
                if (_comparer.Compare(_heap[index].Priority, _heap[parentIndex].Priority) < 0)
                {
                    Swap(index, parentIndex);
                    index = parentIndex;
                }
                else
                {
                    // 堆性质已满足，退出
                    break;
                }
            }
        }

        /// <summary>
        /// 向下堆化（堆顶元素替换后，从上往下调整）
        /// </summary>
        /// <param name="index">当前元素索引</param>
        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;
            while (true)
            {
                // 左子节点索引：2i+1
                int leftChildIndex = index * 2 + 1;
                // 右子节点索引：2i+2
                int rightChildIndex = index * 2 + 2;
                // 假设当前节点是优先级最高的
                int highestPriorityIndex = index;

                // 比较左子节点和当前节点的优先级
                if (leftChildIndex <= lastIndex && _comparer.Compare(_heap[leftChildIndex].Priority, _heap[highestPriorityIndex].Priority) < 0)
                {
                    highestPriorityIndex = leftChildIndex;
                }

                // 比较右子节点和当前最高优先级节点的优先级
                if (rightChildIndex <= lastIndex && _comparer.Compare(_heap[rightChildIndex].Priority, _heap[highestPriorityIndex].Priority) < 0)
                {
                    highestPriorityIndex = rightChildIndex;
                }

                // 如果最高优先级节点不是当前节点，交换并继续调整
                if (highestPriorityIndex != index)
                {
                    Swap(index, highestPriorityIndex);
                    index = highestPriorityIndex;
                }
                else
                {
                    // 堆性质已满足，退出
                    break;
                }
            }
        }

        /// <summary>
        /// 交换堆中两个位置的元素
        /// </summary>
        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }
        #endregion

        #region 枚举器实现
        public IEnumerator<T> GetEnumerator()
        {
            // 遍历前先复制一份，避免遍历过程中堆结构变化导致异常
            var copy = new List<(T Item, int Priority)>(_heap);
            while (copy.Count > 0)
            {
                // 模拟出队顺序遍历（注意：原队列不会被修改）
                var top = copy[0];
                yield return top.Item;

                int lastIdx = copy.Count - 1;
                copy[0] = copy[lastIdx];
                copy.RemoveAt(lastIdx);
                if (copy.Count > 0)
                    HeapifyDown(copy, 0, _comparer);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // 辅助：针对副本的向下堆化
        private void HeapifyDown(List<(T Item, int Priority)> heap, int index, IComparer<int> comparer)
        {
            int lastIndex = heap.Count - 1;
            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int highest = index;

                if (left <= lastIndex && comparer.Compare(heap[left].Priority, heap[highest].Priority) < 0)
                    highest = left;
                if (right <= lastIndex && comparer.Compare(heap[right].Priority, heap[highest].Priority) < 0)
                    highest = right;

                if (highest != index)
                {
                    (heap[index], heap[highest]) = (heap[highest], heap[index]);
                    index = highest;
                }
                else
                    break;
            }
        }
        #endregion
    }
}
