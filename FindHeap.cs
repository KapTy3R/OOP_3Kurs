using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Task4_2
{
    internal class FindHeap
    {
        // Метод для поиска элемента в массиве с использованием минимальной кучи
        public static Buyers FindElementUsingHeap(List<Buyers> array, int target)
        {
            if (array == null || array.Count == 0)
            {
                throw new ArgumentException("Массив не может быть пустым.");
            }

            // Используем PriorityQueue для минимальной кучи
            PriorityQueue<Buyers, int> minHeap = new PriorityQueue<Buyers, int>();

            // Добавляем все элементы массива в кучу
            foreach (var num in array)
            {
                minHeap.Enqueue(num, num.id); // Приоритет равен значению
            }

            // Пока куча не пуста, извлекаем элементы
            while (minHeap.Count > 0)
            {
                Buyers current = minHeap.Dequeue();
                if (current.id == target)
                {
                    return current; // Элемент найден
                }
            }

            return null; // Элемент не найден
        }
        public static Items FindElementUsingHeap(List<Items> array, int target)
        {
            if (array == null || array.Count == 0)
            {
                throw new ArgumentException("Массив не может быть пустым.");
            }

            // Используем PriorityQueue для минимальной кучи
            PriorityQueue<Items, int> minHeap = new PriorityQueue<Items, int>();

            // Добавляем все элементы массива в кучу
            foreach (var num in array)
            {
                minHeap.Enqueue(num, num.id); // Приоритет равен значению
            }

            // Пока куча не пуста, извлекаем элементы
            while (minHeap.Count > 0)
            {
                Items current = minHeap.Dequeue();
                if (current.id == target)
                {
                    return current; // Элемент найден
                }
            }

            return null; // Элемент не найден
        }
        public static Orders FindElementUsingHeap(List<Orders> array, int target)
        {
            MessageBox.Show("OR");
            if (array == null || array.Count == 0)
            {
                throw new ArgumentException("Массив не может быть пустым.");
            }

            // Используем PriorityQueue для минимальной кучи
            PriorityQueue<Orders, int> minHeap = new PriorityQueue<Orders, int>();

            // Добавляем все элементы массива в кучу
            foreach (var num in array)
            {
                minHeap.Enqueue(num, num.id); // Приоритет равен значению
            }

            // Пока куча не пуста, извлекаем элементы
            while (minHeap.Count > 0)
            {
                Orders current = minHeap.Dequeue();
                if (current.id == target)
                {
                    return current; // Элемент найден
                }
            }

            return null; // Элемент не найден
        }


    }

}
