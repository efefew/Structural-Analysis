using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
public static class Extentions
{
    public delegate void BinaryWriteHandler(ref BinaryWriter writer);
    public delegate void BinaryReadHandler(ref BinaryReader reader);
    private static System.Random random = new((int)DateTime.Now.Ticks & 0x0000FFFF);
    #region Methods

    /// <summary>
    /// Перемешивание двух списков однаково
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list1">список 1</param>
    /// <param name="list2">список 2</param>
    public static void MixingTwoLists<T>(IList<T> list1, IList<T> list2)
    {
        if (list1.Count != list2.Count)
            throw new Exception("списки должны иметь одинаковый размер");
        int n = list1.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list1[n], list1[k]) = (list1[k], list1[n]);
            (list2[n], list2[k]) = (list2[k], list2[n]);
        }
    }
    public static void WriteArray(this BinaryWriter writer, double[] arr)
    {
        writer.Write(arr.Length);

        for (int id = 0; id < arr.Length; id++)
            writer.Write(arr[id]);
    }
    public static void WriteArray2D(this BinaryWriter writer, double[,] arr)
    {
        writer.Write(arr.GetLength(1));
        writer.Write(arr.GetLength(0));

        for (int y = 0; y < arr.GetLength(1); y++)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
                writer.Write(arr[x, y]);
        }
    }
    public static double[] ReadArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        double[] array = new double[length];

        for (int id = 0; id < length; id++)
            array[id] = reader.ReadDouble();

        return array;
    }
    public static double[,] ReadArray2D(this BinaryReader reader)
    {
        int yCount = reader.ReadInt32();
        int xCount = reader.ReadInt32();
        double[,] array = new double[xCount, yCount];
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
                array[x, y] = reader.ReadDouble();
        }

        return array;
    }
    public static void WriteDat(string fileName, BinaryWriteHandler write)
    {
        using FileStream fs = new($"{fileName}.dat", FileMode.Create);
        BinaryWriter binaryWriter = new(fs);
        write?.Invoke(ref binaryWriter);
        binaryWriter.Flush();
        binaryWriter.Close();
    }
    public static void ReadDat(string path, BinaryReadHandler read)
    {
        path = Path.GetExtension(path) == ".dat" ? path : $"{path}.dat";
        if (!File.Exists(path))
        {
            Debug.LogWarning($"не существует файла {path}");
            return;
        }

        using FileStream fs = new(path, FileMode.Open);
        BinaryReader binaryReader = new(fs);
        read?.Invoke(ref binaryReader);
        binaryReader.Close();
    }

    /// <summary>
    /// Таймер
    /// </summary>
    /// <param name="timer">значение таймера</param>
    /// <returns>значение таймера равно нулю и не изменилось?</returns>
    public static bool Timer(this ref float timer)
    {
        if (timer == 0)
            return true;

        timer -= Time.fixedDeltaTime;
        if (timer < 0)
            timer = 0;
        return false;
    }

    /// <summary>
    /// Попробовать получить значение другого типа
    /// </summary>
    /// <typeparam name="T">другой тип</typeparam>
    /// <param name="obj">исходное значение</param>
    /// <param name="valueOtherType">значение другого типа</param>
    /// <returns>Получилось ли получить значение другого типа</returns>
    public static bool TryGetValueOtherType<T>(this object obj, out T valueOtherType)
    {
        if (obj.GetType() == typeof(T))
        {
            valueOtherType = (T)obj;
            return true;
        }

        valueOtherType = default;
        return false;
    }

    public static DateTimeOffset ToDateTimeOffset(this string text)
    {
        string[] date = text.Split(' ')[0].Split('.');
        string[] clock = text.Split(' ')[1].Split(':');
        string[] offset = text.Split(' ')[2].Split(':');
        return new DateTimeOffset(
            Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0]),
            Convert.ToInt32(clock[0]), Convert.ToInt32(clock[1]), Convert.ToInt32(clock[2]),
            new TimeSpan(Convert.ToInt32(offset[0]), Convert.ToInt32(offset[1]), 0));
    }

    /// <summary>
    /// Ищет min x, min y, max x, max y
    /// </summary>
    /// <param name="points">точки</param>
    /// <returns>min x, min y, max x, max y</returns>
    public static (float, float, float, float) MinMax(this Vector2[] points)
    {
        Vector2 minPoint = new()
        { x = points[0].x, y = points[0].y };
        Vector2 maxPoint = new()
        { x = points[0].x, y = points[0].y };
        for (int id = 0; id < points.Length; id++)
        {
            if (points[id].x < minPoint.x)
                minPoint.x = points[id].x;
            if (points[id].y < minPoint.y)
                minPoint.y = points[id].y;

            if (points[id].x > maxPoint.x)
                maxPoint.x = points[id].x;
            if (points[id].y > maxPoint.y)
                maxPoint.y = points[id].y;
        }

        return (minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
    }
    #endregion Methods
}
public static class Vector3Extensions
{
    public static Vector3 X(this Vector3 vector, float value) => new(value, vector.y, vector.z);
    public static Vector3 Y(this Vector3 vector, float value) => new(vector.x, value, vector.z);
    public static Vector3 Z(this Vector3 vector, float value) => new(vector.x, vector.y, value);
    public static Vector3 AddX(this Vector3 vector, float x) => new(vector.x + x, vector.y, vector.z);
    public static Vector3 AddY(this Vector3 vector, float y) => new(vector.x, vector.y + y, vector.z);
    public static Vector3 AddZ(this Vector3 vector, float z) => new(vector.x, vector.y, vector.z + z);
    public static Vector3 MulX(this Vector3 vector, float x) => new(vector.x * x, vector.y, vector.z);
    public static Vector3 MulY(this Vector3 vector, float y) => new(vector.x, vector.y * y, vector.z);
    public static Vector3 MulZ(this Vector3 vector, float z) => new(vector.x, vector.y, vector.z * z);
}
public static class ArrayExtensions
{
    private static System.Random random = new((int)DateTime.Now.Ticks & 0x0000FFFF);
    /// <summary>
    /// метод визуализации массива
    /// </summary> 
    /// <returns>строка визуализации</returns>
    public static string ShowArray<T>(this T[] arr, string separator = "\t")
    {
        if (arr.Length == 0)
            return null;
        string str = "";
        str += arr[0];
        if (arr.Length > 1)
        {
            for (int i = 1; i < arr.Length; i++)
                str += separator + arr[i];
        }

        return str;
    }
    /// <summary>
    /// метод визуализации 2D массива
    /// </summary>
    /// <returns>строка визуализации</returns>
    public static string ShowArray<T>(this T[,] arr)
    {
        string str = "";
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
                str += arr[x, y] + "\t";
            str += "\n";
        }

        return str;
    }
    /// <summary>
    /// Нормализовать массив
    /// </summary>
    /// <param name="array">массив</param>
    public static void Normalize(this double[] array, double min = 0, double max = 1)
    {
        double minInArray = array.Min();
        double maxInArray = array.Max();
        if (minInArray == maxInArray)
        {
            double value = maxInArray > 0 ? 1 : 0;
            for (int id = 0; id < array.Length; id++)
                array[id] = value;
            return;
        }

        double relativeValue;
        for (int id = 0; id < array.Length; id++)
        {
            relativeValue = (array[id] - minInArray) / (maxInArray - minInArray);//от 0 до 1
            array[id] = (relativeValue * (max - min)) + min;
        }
    }
    /// <summary>
    /// Перемешивание списка
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">список</param>
    public static void Mixing<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    /// <summary>
    /// Перемешивание массива
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">массив</param>
    public static void Mixing<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
    public static T[] ToArray<T>(this T[,] matrix)
    {
        T[] array = new T[matrix.GetLength(0) * matrix.GetLength(1)];
        int id = 0;
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                array[id] = matrix[x, y];
                id++;
            }
        }

        return array;
    }
    /// <summary>
    /// Нарастить границы матрице заполнив значением value
    /// </summary>
    /// <param name="matrix">массив</param>
    /// <param name="value">значение</param>
    /// <returns>массив с наращенными границами</returns>
    public static bool[,] AddBorders(this bool[,] matrix, bool value = true)
    {
        bool[,] newMatrix = new bool[matrix.GetLength(0) + 2, matrix.GetLength(1) + 2];
        for (int x = 0; x < newMatrix.GetLength(0); x++)
        {
            newMatrix[x, 0] = value;
            newMatrix[x, newMatrix.GetLength(0) - 1] = value;
        }

        for (int y = 0; y < newMatrix.GetLength(1); y++)
        {
            newMatrix[0, y] = value;
            newMatrix[newMatrix.GetLength(1) - 1, y] = value;
        }

        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                newMatrix[x + 1, y + 1] = matrix[x, y];
            }
        }

        return newMatrix;
    }
    public static bool IsEmptyRow(this int[,] matrix, int row)
    {
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
            if (matrix[row, y] != 0)
                return false;
        }

        return true;
    }
    public static bool IsEmptyColumn(this int[,] matrix, int column)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            if (matrix[x, column] != 0)
                return false;
        }

        return true;
    }
    public static int[,] RowRemove(this int[,] matrix, int rowToRemove)
    {
        int[,] result = new int[matrix.GetLength(0) - 1, matrix.GetLength(1)];

        for (int xOriginal = 0, xResult = 0; xOriginal < matrix.GetLength(0); xOriginal++)
        {
            if (xOriginal == rowToRemove)
                continue;

            for (int yOriginal = 0, yResult = 0; yOriginal < matrix.GetLength(1); yOriginal++)
            {
                result[xResult, yResult] = matrix[xOriginal, yOriginal];
                yResult++;
            }

            xResult++;
        }

        return result;
    }
    public static int[,] ColumnRemove(this int[,] matrix, int columnToRemove)
    {
        int[,] result = new int[matrix.GetLength(0), matrix.GetLength(1) - 1];

        for (int xOriginal = 0, xResult = 0; xOriginal < matrix.GetLength(0); xOriginal++)
        {
            for (int yOriginal = 0, yResult = 0; yOriginal < matrix.GetLength(1); yOriginal++)
            {
                if (yOriginal == columnToRemove)
                    continue;

                result[xResult, yResult] = matrix[xOriginal, yOriginal];
                yResult++;
            }

            xResult++;
        }

        return result;
    }
    public static T[,] Transposition<T>(this T[,] arr)
    {
        T[,] newArr = new T[arr.GetLength(1), arr.GetLength(0)];
        for (int x = 0; x < newArr.GetLength(0); x++)
        {
            for (int y = 0; y < newArr.GetLength(1); y++)
                newArr[x, y] = arr[y, x];
        }

        return newArr;
    }
    /// <summary>
    /// умножение матриц
    /// </summary>
    /// <param name="A">матрица A</param>
    /// <param name="B">матрица B</param>
    /// <returns>матрица С = A * B</returns>
    /// <exception cref="Exception">если число столбцов матрицы A не равно числу строк B</exception>
    public static double[,] Multiplication(this double[,] A, double[,] B)
    {
        if (A.GetLength(1) != B.GetLength(0))
            throw new Exception("число столбцов матрицы A не равно числу строк B");
        double[,] C = new double[A.GetLength(0), B.GetLength(1)];
        for (int x = 0; x < C.GetLength(0); x++)
        {
            for (int y = 0; y < C.GetLength(1); y++)
            {
                for (int index = 0; index < A.GetLength(0); index++)
                {
                    C[x, y] += A[x, index] * B[index, y];
                }
            }
        }

        return C;
    }
    /// <summary>
    /// Умножение матрицы на число
    /// </summary>
    /// <param name="A">матрица</param>
    /// <param name="number">число</param>
    /// <returns>матрица B = number * A</returns>
    public static double[,] Multiplication(this double[,] A, double number)
    {
        double[,] B = new double[A.GetLength(0), A.GetLength(1)];
        for (int x = 0; x < B.GetLength(0); x++)
        {
            for (int y = 0; y < B.GetLength(1); y++)
                B[x, y] = A[x, y] * number;
        }

        return B;
    }
    public static double[] Multiplication(this double[] A, double number)
    {
        double[] B = new double[A.Length];
        for (int id = 0; id < B.Length; id++)
            B[id] = A[id] * number;
        return B;
    }
    /// <summary>
    /// сложение матриц
    /// </summary>
    /// <param name="A">матрица A</param>
    /// <param name="B">матрица B</param>
    /// <returns>матрица С = A + B</returns>
    /// <exception cref="Exception">если матрицы A и B не одинаковых размеров</exception>
    public static double[,] Add(this double[,] A, double[,] B)
    {
        if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            throw new Exception("матрицы A и B не одинаковых размеров");
        double[,] C = new double[A.GetLength(0), A.GetLength(1)];
        for (int x = 0; x < C.GetLength(0); x++)
        {
            for (int y = 0; y < C.GetLength(1); y++)
                C[x, y] = A[x, y] + B[x, y];
        }

        return C;
    }
    /// <summary>
    /// вычитание матриц
    /// </summary>
    /// <param name="A">матрица A</param>
    /// <param name="B">матрица B</param>
    /// <returns>матрица С = A - B</returns>
    /// <exception cref="Exception">если матрицы A и B не одинаковых размеров</exception>
    public static double[,] Subtract(this double[,] A, double[,] B)
    {
        if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            throw new Exception("матрицы A и B не одинаковых размеров");
        double[,] C = new double[A.GetLength(0), A.GetLength(1)];
        for (int x = 0; x < C.GetLength(0); x++)
        {
            for (int y = 0; y < C.GetLength(1); y++)
                C[x, y] = A[x, y] - B[x, y];
        }

        return C;
    }
    /// <summary>
    /// Построение матрицы комплексов C
    /// </summary>
    /// <param name="A">матрица A</param>
    /// <param name="B">матрица B</param>
    /// <returns>матрица комплексов С = логически перемножая элементы матриц A и B</returns>
    /// <exception cref="Exception">если матрицы A и B не одинаковых размеров</exception>
    public static double[,] Complex(this double[,] A, double[,] B)
    {
        if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            throw new Exception("матрицы A и B не одинаковых размеров");
        double[,] C = new double[A.GetLength(0), A.GetLength(1)];
        for (int x = 0; x < C.GetLength(0); x++)
        {
            for (int y = 0; y < C.GetLength(1); y++)
                C[x, y] = A[x, y] * B[x, y];
        }

        return C;
    }
    /// <summary>
    /// Создание массива с нумированием строк и столбцов
    /// </summary>
    /// <typeparam name="T">тип элементов массива</typeparam>
    /// <param name="arr">массив</param>
    /// <param name="rows">имена строк</param>
    /// <param name="colomns">имена столбцов</param>
    /// <returns>массив с нумированием строк и столбцов</returns>
    public static string[,] CreateTable<T>(this T[,] arr, out string[] rows, out string[] colomns)
    {
        rows = new string[arr.GetLength(0)];
        for (int i = 0; i < arr.GetLength(0); i++)
            rows[i] = (i + 1).ToString();
        colomns = new string[arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(1); i++)
            colomns[i] = (i + 1).ToString();
        string[,] strArr = new string[arr.GetLength(0) + 1, arr.GetLength(1) + 1];
        strArr[0, 0] = "*";
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            strArr[x + 1, 0] = rows[x];
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                strArr[0, y + 1] = colomns[y];
                strArr[x + 1, y + 1] = arr[x, y].ToString();
            }
        }

        return strArr;
    }
    /// <summary>
    /// Создание массива с именами строк и столбцов
    /// </summary>
    /// <typeparam name="T">тип элементов массива</typeparam>
    /// <param name="arr">массив</param>
    /// <param name="rows">имена строк</param>
    /// <param name="colomns">имена столбцов</param>
    /// <returns>массив с именами строк и столбцов</returns>
    /// <exception cref="Exception"></exception>
    public static string[,] CreateTable<T>(this T[,] arr, string[] rows, string[] colomns)
    {
        if (arr.GetLength(0) != rows.Length)
            throw new Exception("количество номеров строк не совпадает");
        if (arr.GetLength(1) != colomns.Length)
            throw new Exception("количество номеров столбцов не совпадает");
        string[,] strArr = new string[arr.GetLength(0) + 1, arr.GetLength(1) + 1];
        strArr[0, 0] = "*";
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            strArr[x + 1, 0] = rows[x];
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                strArr[0, y + 1] = colomns[y];
                strArr[x + 1, y + 1] = arr[x, y].ToString();
            }
        }

        return strArr;
    }
    /// <summary>
    /// Создание массива с нумированием
    /// </summary>
    /// <typeparam name="T">тип элементов массива</typeparam>
    /// <param name="arr">массив</param>
    /// <param name="names">имена</param>
    /// <returns>массив с нумированием</returns>
    public static string[,] CreateTable<T>(this T[,] arr, out string[] names)
    {
        names = new string[arr.GetLength(0)];
        for (int i = 0; i < arr.GetLength(0); i++)
            names[i] = (i + 1).ToString();
        string[,] strArr = new string[arr.GetLength(0) + 1, arr.GetLength(1) + 1];
        strArr[0, 0] = "*";
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            strArr[x + 1, 0] = names[x];
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                strArr[0, y + 1] = names[y];
                strArr[x + 1, y + 1] = arr[x, y].ToString();
            }
        }

        return strArr;
    }
    /// <summary>
    /// Создание массива с именами
    /// </summary>
    /// <typeparam name="T">тип элементов массива</typeparam>
    /// <param name="arr">массив</param>
    /// <param name="names">имена</param>
    /// <returns>массив с именами</returns>
    /// <exception cref="Exception"></exception>
    public static string[,] CreateTable<T>(this T[,] arr, string[] names)
    {
        if (arr.GetLength(0) != names.Length)
            throw new Exception("количество номеров строк не совпадает");
        if (arr.GetLength(1) != names.Length)
            throw new Exception("количество номеров столбцов не совпадает");
        string[,] strArr = new string[arr.GetLength(0) + 1, arr.GetLength(1) + 1];
        strArr[0, 0] = "*";
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            strArr[x + 1, 0] = names[x];
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                strArr[0, y + 1] = names[y];
                strArr[x + 1, y + 1] = arr[x, y].ToString();
            }
        }

        return strArr;
    }
    public static bool Contains<T>(this T[] arr, T value)
    {
        if (value == null)
            throw new Exception("value должно быть не равно null");
        for (int i = 0; i < arr.Length; i++)
        {
            if (value.Equals(arr[i]))
                return true;
        }

        return false;
    }
}
public static class UnityExtensions
{
    public static Sprite ToSprite(this Texture2D tex) => Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    public static Texture2D ToTexture2D(this byte[] bytes)
    {
        Texture2D texture = new(2, 2);
        _ = texture.LoadImage(bytes);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// Очистить объект от вложенных объектов
    /// </summary>
    /// <param name="transform">объект</param>
    public static void Clear(this Transform transform)
    {
        if (transform.childCount == 0)
            return;
        for (int idChild = 0; idChild < transform.childCount; idChild++)
            UnityEngine.Object.Destroy(transform.GetChild(idChild).gameObject);
    }

    /// <summary>
    /// Следить за целью (2D версия)
    /// </summary>
    /// <param name="transform">следящий</param>
    /// <param name="target">цель</param>
    public static void LookAt2D(this Transform transform, Vector3 target)
    {
        float angle = transform.position.Angle(target);
        transform.eulerAngles = transform.eulerAngles.Z(angle);
    }
    public static float Angle(this Vector3 position, Vector3 target)
    {
        Vector2 direction = target - position;
        return Vector2.SignedAngle(Vector2.right, direction);
    }
    /// <summary>
    /// Проверяет, находится ли указатель мыши над объектом UI.
    /// </summary>
    /// <returns>находится ли указатель мыши над объектом UI</returns>
    public static bool IsPointerOverUI()
    {
        // Создаем экземпляр PointerEventData с текущим положением указателя мыши.
        PointerEventData eventDataCurrentPosition = new(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };

        List<RaycastResult> results = new();

        // Выполняем лучевой кастинг для всех объектов в текущей позиции указателя мыши.
        // Результаты сохраняются в списке results.
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Если количество результатов больше нуля, значит указатель мыши находится над объектом UI.
        return results.Count > 0;
    }
}
public static class ConsoleExtensions
{
    public static void Warning(string text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
    /// <summary>
    /// Создаёт массив с помощью пользователя
    /// </summary>
    /// <returns>массив</returns>
    public static float[] PrintArray()
    {
        float[] arr;
        int size;
        Console.Write("Введите длину массива: ");

        try
        {
            size = Console.ReadLine().ToInt();
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ForegroundColor = default;
            return null;
        }

        arr = new float[size];
        for (int id = 0; id < size; id++)
        {
            Console.WriteLine("Введите элемент " + (id + 1));
            try
            {
                arr[id] = Console.ReadLine().ToInt();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = default;
                return null;
            }
        }

        return arr;
    }
    /// <summary>
    /// Создаёт 2D массив с помощью пользователя
    /// </summary>
    /// <returns>2D массив</returns>
    public static float[,] Print2DArray()
    {
        float[,] arr;
        int size;
        Console.Write("Введите длину массива: ");

        try
        {
            size = Console.ReadLine().ToInt();
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ForegroundColor = default;
            return null;
        }

        arr = new float[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool again = true;
                while (again)
                {
                    Console.WriteLine("Введите элемент " + (x + 1) + ", " + (y + 1));
                    string line = Console.ReadLine();
                    try
                    {
                        arr[x, y] = line.ToInt();
                        again = false;
                    }
                    catch (Exception ex)
                    {
                        if (!(x == 0 && y == 0) && line == "")
                        {
                            Console.WriteLine("вы вернулись назад");
                            y--;
                            if (y < 0)
                            {
                                y = 0;
                                x--;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex);
                            Console.ForegroundColor = default;
                            Console.WriteLine("повторите ещё раз");
                        }
                    }
                }
            }
        }

        return arr;
    }
}
public static class ConvertExtensions
{
    public static float[] ToPercent(this float[] arr)
    {
        int length = arr.Length;
        float summ = 0;
        for (int i = 0; i < length; i++)
            summ += arr[i];
        for (int i = 0; i < length; i++)
            arr[i] *= 100f / summ;
        return arr;
    }
    public static float ToFloat(this string s) => Convert.ToSingle(s.Replace('.', ','));

    public static float[] ToFloat(this string[] s)
    {
        float[] sFloat = new float[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            sFloat[i] = Convert.ToSingle(s[i].Replace('.', ','));
        }

        return sFloat;
    }
    public static float[] ToFloat(this int[] arr)
    {
        float[] arrFloat = new float[arr.Length];
        for (int i = 0; i < arr.Length; i++)
            arrFloat[i] = arr[i];
        return arrFloat;
    }
    public static int ToInt(this string s) => Convert.ToInt32(s);
    public static int[] ToInt(this string[] s)
    {
        int[] sInt = new int[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            sInt[i] = Convert.ToInt32(s[i]);
        }

        return sInt;
    }
    public static bool ToBool(this string s) => s is "true" or "1";
    public static bool[] ToArrBool(this string s)
    {
        bool[] Be = new bool[s.Length];
        for (int i = 0; i < s.Length; i++)
            Be[i] = s[i] == '1';
        return Be;
    }
}