using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class StructuralAnalysisCTS
{
    private (int, int[], double[,]) FindEmptyColumnAndDelete(double[,] arr, int[] idArr)
    {
        int id = -1;
        for (int y = 0; y < arr.GetLength(1); y++)
        {
            bool AllCellsZero = true;
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                if (arr[x, y] == 1)
                {
                    AllCellsZero = false;
                    break;
                }
            }

            if (AllCellsZero)
            {
                id = y;
                break;
            }
        }

        double[,] newArr = new double[arr.GetLength(0) - 1, arr.GetLength(1) - 1];
        int[] newIdArr = new int[idArr.Length - 1];
        int addX = 0;
        for (int x = 0; x < arr.GetLength(0) - 1; x++)
        {
            if (id == x)
                addX++;
            newIdArr[x] = idArr[x + addX];
            int addY = 0;
            for (int y = 0; y < arr.GetLength(1) - 1; y++)
            {
                if (id == y)
                    addY++;
                newArr[x, y] = arr[x + addX, y + addY];
            }
        }

        return (idArr[id], newIdArr, newArr);
    }

    private void SearchLine(ref double[,] newArr, double[,] arr, int targetPoint, int curentPoint = 0)
    {
        if (newArr[targetPoint, curentPoint] == 0)
            return;
        int length = arr.GetLength(0);
        for (int id = 0; id < length; id++)
        {
            if (arr[curentPoint, id] != 0 && newArr[targetPoint, id] == 0)
            {
                newArr[targetPoint, id] = arr[curentPoint, id];
                SearchLine(ref newArr, arr, targetPoint, id);
            }
        }
    }

    private double[,] ConvertToUgaBuga(double[,] arr)
    {
        if (arr.GetLength(0) != arr.GetLength(1))
            throw new Exception("массив должен иметь одинаковые размеры по двум ос€м");
        int length = arr.GetLength(0);
        //double[] line = new double[length];
        double[,] newArr = new double[length, length];
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if (arr[x, y] > 0)
                    newArr[x, y] = arr[x, y];//за один шаг
                if (x == y)
                    newArr[x, y] = 1;
            }

            for (int y = 0; y < length; y++)
                SearchLine(ref newArr, arr, x, y);//за несколько шагов
        }

        return newArr;
    }

    private (double[,], string[][]) CutEmptyColumns(double[,] arr, double[,] adjacencyMatrix, ref string[] names)
    {
        List<List<string>> complexes = new();
        List<string> tempNames = new();
        tempNames.AddRange(names);
        bool cut;
        for (int idRow = 0; idRow < arr.GetLength(0); idRow++)
        {
            cut = true;
            for (int idColomn = 0; idColomn < arr.GetLength(1); idColomn++)
            {
                if (arr[idRow, idColomn] == 1 && idRow != idColomn)
                {
                    cut = false;
                    break;
                }
            }

            if (cut)
                tempNames.RemoveAt(tempNames.IndexOf(names[idRow]));
        }

        double[,] cutAdjacencyMatrix = new double[tempNames.Count, tempNames.Count];

        for (int idRow = 0; idRow < arr.GetLength(0); idRow++)
        {
            for (int idColomn = 0; idColomn < arr.GetLength(1); idColomn++)
            {
                if (tempNames.Contains(names[idRow]) && tempNames.Contains(names[idColomn]))
                {
                    int idR = tempNames.IndexOf(names[idRow]);
                    int idC = tempNames.IndexOf(names[idColomn]);
                    cutAdjacencyMatrix[idR, idC] = adjacencyMatrix[idRow, idColomn];
                }
            }
        }

        {
            bool first = true, allZero;
            List<string> usedNames = new();
            for (int idRow = 0; idRow < arr.GetLength(0) - 1; idRow++)
            {
                allZero = true;
                if (complexes.Count == 0 || !usedNames.Contains(names[idRow]))
                {
                    for (int idColomn = idRow + 1; idColomn < arr.GetLength(1); idColomn++)
                    {
                        if (arr[idRow, idColomn] == 1 && idColomn != idRow)
                        {
                            allZero = false;
                            if (first)
                            {
                                complexes.Add(new List<string>());
                                usedNames.Add(names[idRow]);
                                complexes[^1].Add(names[idRow]);
                                first = false;
                            }

                            complexes[^1].Add(names[idColomn]);
                            usedNames.Add(names[idColomn]);
                        }
                    }
                }

                if (allZero)
                    first = true;
            }
        }

        names = tempNames.ToArray();
        return (cutAdjacencyMatrix, To2DArray(complexes));
    }

    private string[][] To2DArray(List<List<string>> list2D)
    {
        string[][] arr2D;
        arr2D = new string[list2D.Count][];
        for (int i = 0; i < list2D.Count; i++)
        {
            arr2D[i] = new string[list2D[i].Count];
            for (int j = 0; j < list2D[i].Count; j++)
                arr2D[i][j] = list2D[i][j];
        }

        return arr2D;
    }
    private string[,] To2DArray(List<string[]> list2D)
    {
        string[,] arr2D;
        arr2D = new string[list2D.Count, list2D[0].Length];
        for (int i = 0; i < list2D.Count; i++)
        {
            for (int j = 0; j < list2D[0].Length; j++)
                arr2D[i, j] = list2D[i][j];
        }

        return arr2D;
    }

    private (string[,], string[][]) CreateConnectionsAndContours(double[,] cutAdjacencyMatrix, string[] names)
    {
        List<string[]> connections = new();
        List<List<string>> contours = new();
        for (int x = 0; x < cutAdjacencyMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < cutAdjacencyMatrix.GetLength(1); y++)
            {
                if (cutAdjacencyMatrix[x, y] == 1)
                    connections.Add(new string[2] { names[x], names[y] });
            }
        }

        Debug.Log("соединени€:");
        for (int i = 0; i < connections.Count; i++)
            Debug.Log(connections[i][0] + "-" + connections[i][1]);

        List<string> tempContour = new()
            {
                connections[0][0]
            };
        bool forward = true;
        Debug.Log("-------------------------------------");
        do
        {
            string nextElement;
            if (forward)
            {
                nextElement = FirstFindElementConnexion(tempContour[^1], connections);
            }
            else
            {
                nextElement = NextFindElementConnexion(tempContour[^1], tempContour[^2], connections);
                tempContour.RemoveAt(tempContour.Count - 1);
            }

            if (nextElement != null)
            {
                if (tempContour.Contains(nextElement))
                {
                    forward = false;
                    tempContour.Add(nextElement);
                    contours.Add(new List<string>());
                    contours[^1].AddRange(tempContour);
                    contours[^1].RemoveRange(0, tempContour.IndexOf(nextElement));

                }
                else
                {
                    forward = true;
                    tempContour.Add(nextElement);
                }
            }
            else
            {
                if (tempContour.Count == 1)
                    break;
                forward = false;
            }
            //Debug.Log(tempContour.ToArray().ShowArray());
        }
        while (tempContour.Count > 0);//находит повторени€ дл€ контуров

        DeleteRepeatContours(contours);
        for (int i = 0; i < contours.Count; i++)
            Debug.Log("<color=#22EE22>" + contours[i].ToArray().ShowArray("-") + "</color>");

        return (To2DArray(connections), To2DArray(contours));
    }

    private void DeleteRepeatContours(List<List<string>> contours)
    {
        for (int i = 0; i < contours.Count; i++)
        {
            for (int j = 0; j < contours.Count; j++)
            {
                if (i != j && contours[i].Count == contours[j].Count)
                {
                    if (RepeatContour(contours[i], contours[j]) || RepeatDisplacedContour(contours[i], contours[j]))
                    {
                        Debug.Log("<color=#22EED4>" + contours[i].ToArray().ShowArray("-") + "</color>");
                        _ = contours.Remove(contours[i]);
                        break;
                    }
                }
            }
        }
    }

    private bool RepeatDisplacedContour(List<string> contour1, List<string> contour2)
    {
        List<string> c1 = new();
        c1.AddRange(contour1);
        c1.RemoveAt(0);
        List<string> c2 = new();
        c2.AddRange(contour2);
        c2.RemoveAt(0);
        int displacedId;
        for (int i = 0; i < c1.Count; i++)
        {
            displacedId = i;
            for (int j = 0; j < c2.Count; j++)
            {
                displacedId++;
                if (displacedId == c2.Count)
                    displacedId = 0;
                if (c1[displacedId] != c2[j])
                    break;
                if (j + 1 == c2.Count)
                    return true;
            }
        }

        return false;
    }

    private bool RepeatContour(List<string> contour1, List<string> contour2)
    {
        for (int i = 0; i < contour1.Count; i++)
        {
            if (contour1[i] != contour2[i])
                return false;
        }

        return true;
    }
    private string FirstFindElementConnexion(string fromElement, List<string[]> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i][0] == fromElement)
                return connections[i][1];
        }

        return null;
    }
    private string NextFindElementConnexion(string oldToElement, string fromElement, List<string[]> connections)
    {
        bool oldToElementFinded = false;
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i][0] == fromElement)
            {
                if (connections[i][1] == oldToElement)
                {
                    oldToElementFinded = true;
                    continue;
                }

                if (oldToElementFinded)
                    return connections[i][1];
            }
        }

        return null;
    }

    private int ArcInContour(string fromElement, string toElement, string[] contour)
    {
        for (int i = 0; i < contour.Length - 1; i++)
        {
            if (contour[i] == fromElement && contour[i + 1] == toElement)
                return 1;
        }

        return 0;
    }

    private void MatrixOfContoursOfTheComplex(string[][] contours, string[,] connections, double[,] adjacencyMatrix)
    {
        int[,] matrix = new int[contours.Length + 2, connections.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0) - 2; i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
                matrix[i, j] = ArcInContour(connections[j, 0], connections[j, 1], contours[i]);
        }

        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            matrix[matrix.GetLength(0) - 2, i] = 0;
            for (int j = 0; j < matrix.GetLength(0) - 2; j++)
                matrix[matrix.GetLength(0) - 2, i] += matrix[j, i];
            matrix[matrix.GetLength(0) - 1, i] = 1;
        }

        string[] contoursName = new string[contours.Length + 2];
        for (int i = 0; i < contoursName.Length - 2; i++)
        {
            contoursName[i] = contours[i][0];
            for (int j = 1; j < contours[i].Length; j++)
                contoursName[i] += "_" + contours[i][j];
        }

        contoursName[^2] = "f";
        contoursName[^1] = "p";

        string[] connectionsName = new string[connections.GetLength(0)];
        for (int i = 0; i < connectionsName.Length; i++)
        {
            connectionsName[i] = connections[i, 0] + "_" + connections[i, 1];
        }

        StreamWriter writer = new("excel.xls");
        writer.WriteLine(matrix.CreateTable(contoursName, connectionsName).ShowArray());
        writer.Close();
        double[,] openAdjacencyMatrix = new double[adjacencyMatrix.GetLength(0), adjacencyMatrix.GetLength(1)];
        SAOfOpenedTHS(openAdjacencyMatrix);
    }
    /// <summary>
    /// —труктурный анализ химикотехнологических систем
    /// </summary>
    /// <param name="adjacencyMatrix">матрица смежности</param>
    /// <returns></returns>
    public bool StructuralAnalysisChemicalTechnologicalSystems(double[,] adjacencyMatrix)
    {
        if (adjacencyMatrix == null ||
            adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
        {
            return false;
        }

        Debug.Log(adjacencyMatrix.ShowArray());
        double[,] arr = ConvertToUgaBuga(adjacencyMatrix);
        Debug.Log("A = ");
        Debug.Log(arr.ShowArray());
        Debug.Log("A^T = ");
        double[,] arrT = arr.Transposition();
        Debug.Log(arrT.ShowArray());
        Debug.Log("Ћогически перемножа€ элементы матриц A и A^T = ");
        double[,] complexArr = arr.Complex(arrT);
        if (OnlyMainDiagonal(complexArr))
        {
            Debug.Log("“ак как полученна€ матрица имеет единицу только на главной диагонали, то означает, что это разомкнута€ система");
            SAOfOpenedTHS(adjacencyMatrix);
            return false;
        }

        Debug.Log(complexArr.CreateTable(out string[] names).ShowArray());
        double[,] cutAdjacencyMatrix;
        string[][] complexes;
        (cutAdjacencyMatrix, complexes) = CutEmptyColumns(complexArr, adjacencyMatrix, ref names);
        for (int i = 0; i < complexes.Length; i++)
        {
            Debug.Log(" омплекс " + (i + 1) + ":");
            for (int j = 0; j < complexes[i].Length; j++)
                Debug.Log(complexes[i][j]);
        }
        //Debug.Log(names.ShowArray());
        Debug.Log("ѕостроим матрицу смежности:");
        Debug.Log(cutAdjacencyMatrix.CreateTable(names).ShowArray());
        string[][] contours;
        string[,] connections;
        (connections, contours) = CreateConnectionsAndContours(cutAdjacencyMatrix, names);
        MatrixOfContoursOfTheComplex(contours, connections, adjacencyMatrix);
        return true;
    }
    /// <summary>
    /// —труктурный анализ разомкнутых химикотехнологических систем
    /// </summary>
    /// <param name="adjacencyMatrix">матрица смежности</param>
    private void SAOfOpenedTHS(double[,] adjacencyMatrix)
    {
        if (adjacencyMatrix == null)
            return;
        string path = "";
        int size = adjacencyMatrix.GetLength(0);
        int id;
        int[] idArr = new int[size];
        for (int i = 0; i < size; i++)
            idArr[i] = i + 1;
        Debug.Log("<color=#DDEE22>" + idArr.ShowArray() + "</color>");
        Debug.Log(adjacencyMatrix.ShowArray());

        for (int i = 0; i < size; i++)
        {

            (id, idArr, adjacencyMatrix) = FindEmptyColumnAndDelete(adjacencyMatrix, idArr);
            Debug.Log("<color=#DDEE22>" + idArr.ShowArray() + "</color>");
            Debug.Log(adjacencyMatrix.ShowArray());

            if (id == -1)
                continue;
            if (idArr.Length != 0)
                path += " " + id;
            if (idArr.Length == 1)
                path += " " + idArr[0];
        }

        Debug.Log(path);
    }
    private bool OnlyMainDiagonal(double[,] arr)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                if (x != y && arr[x, y] != 0)
                    return false;
            }
        }

        return true;
    }
}