using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
public struct Connection
{
    public string nameFrom, nameTo, name;
    public int idFrom, idTo;

    public Connection(string nameFrom, string nameTo, int idFrom, int idTo)
    {
        this.nameFrom = nameFrom;
        this.nameTo = nameTo;
        this.idFrom = idFrom;
        this.idTo = idTo;
        name = nameFrom + "_" + nameTo;
    }
}
public class Contour
{
    public List<int> idElements = new();
    public List<string> nameElements = new();
}
public class StructuralAnalysisCTS
{
    private (int, int[], int[,]) FindEmptyColumnAndDelete(int[,] arr, int[] idArr)
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

        int[,] newArr = new int[arr.GetLength(0) - 1, arr.GetLength(1) - 1];
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

    private void SearchLine(ref int[,] newArr, int[,] arr, int targetPoint, int curentPoint = 0)
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

    private int[,] ToConnectionMatrix(int[,] arr)
    {
        if (arr.GetLength(0) != arr.GetLength(1))
            throw new Exception("������ ������ ����� ���������� ������� �� ���� ����");
        int length = arr.GetLength(0);
        //double[] line = new double[length];
        int[,] newArr = new int[length, length];
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if (arr[x, y] > 0)
                    newArr[x, y] = arr[x, y];//�� ���� ���
                if (x == y)
                    newArr[x, y] = 1;
            }

            for (int y = 0; y < length; y++)
                SearchLine(ref newArr, arr, x, y);//�� ��������� �����
        }

        return newArr;
    }

    private (double[,], string[][]) CutEmptyColumns(int[,] arr, int[,] adjacencyMatrix, ref string[] names)
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
                if (arr[idRow, idColomn] != 0 && idRow != idColomn)
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

    private (Connection[], Contour[]) CreateConnectionsAndContours(double[,] cutAdjacencyMatrix, string[] names)
    {
        List<Connection> connections = new();
        List<Contour> contours = new();
        for (int x = 0; x < cutAdjacencyMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < cutAdjacencyMatrix.GetLength(1); y++)
            {
                if (cutAdjacencyMatrix[x, y] != 0)
                    connections.Add(new Connection(names[x], names[y], x, y));
            }
        }

        Debug.Log("Соединения:");
        for (int i = 0; i < connections.Count; i++)
            Debug.Log(connections[i].name);

        List<int> tempContour = new()
            {
                connections[0].idFrom
            };
        bool forward = true;
        Debug.Log("-------------------------------------");
        do
        {
            int nextElementID;
            if (forward)
            {
                nextElementID = FirstFindElementConnection(tempContour[^1], connections);
            }
            else
            {
                nextElementID = NextFindElementConnection(tempContour[^1], tempContour[^2], connections);
                tempContour.RemoveAt(tempContour.Count - 1);
            }

            if (nextElementID != -1)
            {
                if (tempContour.Contains(nextElementID))
                {
                    forward = false;
                    tempContour.Add(nextElementID);
                    contours.Add(new Contour());
                    contours[^1].idElements.AddRange(tempContour);
                    contours[^1].idElements.RemoveRange(0, tempContour.IndexOf(nextElementID));
                    for (int id = 0; id < contours[^1].idElements.Count; id++)
                        contours[^1].nameElements.Add(names[contours[^1].idElements[id]]);
                }
                else
                {
                    forward = true;
                    tempContour.Add(nextElementID);
                }
            }
            else
            {
                if (tempContour.Count == 1)
                    break;
                forward = false;
            }
            //Debug.Log(tempContourValues.ToArray().ShowArray());
        }
        while (tempContour.Count > 0);//������� ���������� ��� ��������

        DeleteRepeatContours(contours);
        for (int i = 0; i < contours.Count; i++)
            Debug.Log("<color=#22EE22>" + contours[i].nameElements.ShowList("-") + "</color>");

        return (connections.ToArray(), contours.ToArray());
    }

    private void DeleteRepeatContours(List<Contour> contours)
    {
        for (int i = 0; i < contours.Count; i++)
        {
            for (int j = 0; j < contours.Count; j++)
            {
                if (i != j && contours[i].idElements.Count == contours[j].idElements.Count)
                {
                    if (RepeatContour(contours[i], contours[j]) || RepeatDisplacedContour(contours[i], contours[j]))
                    {
                        Debug.Log("<color=#22EED4>" + contours[i].nameElements.ShowList("-") + "</color>");
                        _ = contours.Remove(contours[i]);
                        break;
                    }
                }
            }
        }
    }

    private bool RepeatDisplacedContour(Contour contour1, Contour contour2)
    {
        List<int> idElements1 = new();
        idElements1.AddRange(contour1.idElements);
        idElements1.RemoveAt(0);
        List<int> idElements2 = new();
        idElements2.AddRange(contour2.idElements);
        idElements2.RemoveAt(0);
        int displacedId;
        for (int i = 0; i < idElements1.Count; i++)
        {
            displacedId = i;
            for (int j = 0; j < idElements2.Count; j++)
            {
                displacedId++;
                if (displacedId == idElements2.Count)
                    displacedId = 0;
                if (idElements1[displacedId] != idElements2[j])
                    break;
                if (j + 1 == idElements2.Count)
                    return true;
            }
        }

        return false;
    }

    private bool RepeatContour(Contour contour1, Contour contour2)
    {
        for (int i = 0; i < contour1.idElements.Count; i++)
        {
            if (contour1.idElements[i] != contour2.idElements[i])
                return false;
        }

        return true;
    }
    private int FirstFindElementConnection(int fromElementID, List<Connection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].idFrom == fromElementID)
                return connections[i].idTo;
        }

        return -1;
    }
    private int NextFindElementConnection(int oldToElementID, int fromElementID, List<Connection> connections)
    {
        bool oldToElementFinded = false;
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].idFrom == fromElementID)
            {
                if (connections[i].idTo == oldToElementID)
                {
                    oldToElementFinded = true;
                    continue;
                }

                if (oldToElementFinded)
                    return connections[i].idTo;
            }
        }

        return -1;
    }

    private int ArcInContour(string fromElement, string toElement, Contour contour)
    {
        for (int i = 0; i < contour.nameElements.Count - 1; i++)
        {
            if (contour.nameElements[i] == fromElement && contour.nameElements[i + 1] == toElement)
                return 1;
        }

        return 0;
    }

    private void MatrixOfContoursOfTheComplex(Contour[] contours, Connection[] connections, int[,] adjacencyMatrix)
    {
        int[,] matrixArcAndContour = new int[contours.Length, connections.GetLength(0)];
        int[] countP = new int[connections.GetLength(0)];
        int[] countArc = new int[connections.GetLength(0)];
        for (int i = 0; i < matrixArcAndContour.GetLength(0); i++)
        {
            for (int j = 0; j < matrixArcAndContour.GetLength(1); j++)
                matrixArcAndContour[i, j] = ArcInContour(connections[j].nameFrom, connections[j].nameTo, contours[i]) * adjacencyMatrix[connections[j].idFrom, connections[j].idTo];
        }

        for (int i = 0; i < matrixArcAndContour.GetLength(0); i++)
        {
            countP[i] = 0;
            countArc[i] = 0;
            for (int j = 0; j < matrixArcAndContour.GetLength(1); j++)
            {
                countP[i] += matrixArcAndContour[j, i];//matrixArcAndContour[j, i];//суммарная параметричность
                if (matrixArcAndContour[j, i] != 0)
                    countArc[i]++;//количество дуг
            }
        }

        string[] contoursName = new string[contours.Length + 2];
        for (int i = 0; i < contoursName.Length - 2; i++)
        {
            contoursName[i] = contours[i].nameElements[0];
            for (int j = 1; j < contours[i].nameElements.Count; j++)
                contoursName[i] += "_" + contours[i].nameElements[j];
        }

        contoursName[^2] = "f";
        contoursName[^1] = "p";

        string[] connectionsName = new string[connections.GetLength(0)];
        for (int i = 0; i < connectionsName.Length; i++)
        {
            connectionsName[i] = connections[i].name;
        }

        using (StreamWriter writer = new("excel.xls"))
        {
            writer.WriteLine(ShowMatrixOfContoursOfTheComplex(contours, connections, matrixArcAndContour, countP, countArc));
        }

        int[,] openAdjacencyMatrix = SplitAdjacencyMatrix(adjacencyMatrix, matrixArcAndContour, countP, countArc, connections, contours);
        SAOfOpenedTHS(openAdjacencyMatrix);
    }
    public string ShowMatrixOfContoursOfTheComplex(Contour[] contours, Connection[] connections, int[,] matrixArcAndContour, int[] countP, int[] countArc)
    {
        int[,] showMatrix = new int[matrixArcAndContour.GetLength(0) + 2, matrixArcAndContour.GetLength(1)];

        //записываем матрицу  присутствия дуг в контурах
        for (int i = 0; i < matrixArcAndContour.GetLength(0); i++)
        {
            for (int j = 0; j < matrixArcAndContour.GetLength(1); j++)
                showMatrix[i, j] = matrixArcAndContour[i, j];
        }

        //записываем параметричность и количество дуг
        for (int i = 0; i < showMatrix.GetLength(1); i++)
        {
            showMatrix[showMatrix.GetLength(0) - 1, i] = countP[i];
            showMatrix[showMatrix.GetLength(0) - 2, i] = countArc[i];
        }

        //записываем имена контуров
        string[] contoursName = new string[contours.Length + 2];
        for (int i = 0; i < contoursName.Length - 2; i++)
        {
            contoursName[i] = contours[i].nameElements[0];
            for (int j = 1; j < contours[i].nameElements.Count; j++)
                contoursName[i] += "_" + contours[i].nameElements[j];
        }

        //записываю наименование количества дуг и параметричности
        contoursName[^2] = "f";
        contoursName[^1] = "p";

        //записываем наименование дуг
        string[] connectionsName = new string[connections.GetLength(0)];
        for (int i = 0; i < connectionsName.Length; i++)
            connectionsName[i] = connections[i].name;

        return showMatrix.CreateTable(contoursName, connectionsName).ShowArray();
    }
    public int[,] SplitAdjacencyMatrix(int[,] adjacencyMatrix, int[,] matrixArcAndContour, int[] countP, int[] countArc, Connection[] connections, Contour[] contours)
    {
        int[,] openAdjacencyMatrix = new int[adjacencyMatrix.GetLength(0), adjacencyMatrix.GetLength(1)];
        for (int x = 0; x < adjacencyMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < adjacencyMatrix.GetLength(1); y++)
            {
                openAdjacencyMatrix[x, y] = adjacencyMatrix[x, y];
            }
        }

        List<int> splitContours = new();
        List<int> usedArcs = new();
        for (int id = 0; id < matrixArcAndContour.GetLength(0); id++)
            splitContours.Add(id);
        int min, idMinArc;

        while (splitContours.Count > 0)
        {
            min = int.MaxValue;
            idMinArc = 0;
            for (int idArc = 0; idArc < countArc.Length; idArc++)
            {
                if (countArc[idArc] != 0 && min > (countP[idArc] - countArc[idArc]) && !usedArcs.Contains(idArc))
                {
                    idMinArc = idArc;
                    min = countP[idArc] - countArc[idArc];
                }
            }

            usedArcs.Add(idMinArc);

            for (int idContour = 0; idContour < matrixArcAndContour.GetLength(0); idContour++)
            {
                if (matrixArcAndContour[idContour, idMinArc] != 0)
                {
                    Debug.Log($"<color=#AC68FA>Разрываем связь {connections[idMinArc].name}</color>");
                    openAdjacencyMatrix[connections[idMinArc].idFrom, connections[idMinArc].idTo] = 0;
                    if (splitContours.Contains(idContour))
                        _ = splitContours.Remove(idContour);
                }
            }
        }

        return openAdjacencyMatrix;
    }
    /// <summary>
    /// ����������� ������ ��������������������� ������
    /// </summary>
    /// <param name="adjacencyMatrix">������� ���������</param>
    /// <returns></returns>
    public bool StructuralAnalysisChemicalTechnologicalSystems(int[,] adjacencyMatrix)
    {
        if (adjacencyMatrix == null ||
            adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
        {
            return false;
        }

        Debug.Log(adjacencyMatrix.ShowArray());
        int[,] connectionMatrix = ToConnectionMatrix(adjacencyMatrix);
        Debug.Log("A = ");
        Debug.Log(connectionMatrix.ShowArray());
        Debug.Log("A^T = ");
        int[,] arrT = connectionMatrix.Transposition();
        Debug.Log(arrT.ShowArray());
        Debug.Log("A * A^T = ");
        int[,] complexArr = connectionMatrix.Complex(arrT);
        if (OnlyMainDiagonal(complexArr))
        {
            Debug.Log("OnlyMainDiagonal");
            SAOfOpenedTHS(adjacencyMatrix);
            return false;
        }

        Debug.Log(complexArr.CreateTable(out string[] names).ShowArray());
        double[,] cutAdjacencyMatrix;
        string[][] complexes;
        (cutAdjacencyMatrix, complexes) = CutEmptyColumns(complexArr, adjacencyMatrix, ref names);
        for (int i = 0; i < complexes.Length; i++)
        {
            Debug.Log("�������� " + (i + 1) + ":");
            for (int j = 0; j < complexes[i].Length; j++)
                Debug.Log(complexes[i][j]);
        }
        //Debug.Log(nameElements.ShowArray());
        Debug.Log("�������� ������� ���������:");
        Debug.Log(cutAdjacencyMatrix.CreateTable(names).ShowArray());
        Contour[] contours;
        Connection[] connections;
        (connections, contours) = CreateConnectionsAndContours(cutAdjacencyMatrix, names);
        MatrixOfContoursOfTheComplex(contours, connections, adjacencyMatrix);
        return true;
    }
    /// <summary>
    /// ����������� ������ ����������� ��������������������� ������
    /// </summary>
    /// <param name="adjacencyMatrix">������� ���������</param>
    private void SAOfOpenedTHS(int[,] adjacencyMatrix)
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
    private bool OnlyMainDiagonal(int[,] arr)
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