using System;
using System.Collections.Generic;

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
    string text;
    private (int, int[], int[,]) FindEmptyColumnAndDelete(int[,] arr, int[] idArr)
    {
        int id = -1;
        for (int y = 0; y < arr.GetLength(1); y++)
        {
            bool AllCellsZero = true;
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                if (arr[x, y] != 0)
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
            throw new Exception("да");
        int length = arr.GetLength(0);
        //double[] line = new double[length];
        int[,] newArr = new int[length, length];
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if (arr[x, y] > 0)
                    newArr[x, y] = arr[x, y];
                if (x == y)
                    newArr[x, y] = 1;
            }

            for (int y = 0; y < length; y++)
                SearchLine(ref newArr, arr, x, y);
        }

        return newArr;
    }

    private (int[,], string[][]) CutEmptyColumns(int[,] arr, int[,] adjacencyMatrix, ref string[] names)
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

        int[,] cutAdjacencyMatrix = new int[tempNames.Count, tempNames.Count];

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
        return (cutAdjacencyMatrix, complexes.To2DArray());
    }

    private (Connection[], Contour[]) CreateConnectionsAndContours(int[,] cutAdjacencyMatrix, string[] names)
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

        //Debug.Log("Соединения:");
        //for (int id = 0; id < connections.Count; id++)
        //    Debug.Log(connections[id].name);

        List<int> tempContour = new()
            {
                connections[0].idFrom
            };
        bool forward = true;
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
        while (tempContour.Count > 0);

        DeleteRepeatContours(contours);
        text += '\n' + "contours:";

        for (int i = 0; i < contours.Count; i++)
        {
            //Debug.Log("<color=#22EE22>" + contours[i].nameElements.ShowList("-") + "</color>");
            text += '\n' + contours[i].nameElements.ShowList("_");
        }

        return (connections.ToArray(), contours.ToArray());
    }

    private void DeleteRepeatContours(List<Contour> contours)
    {
        text += '\n' + "delete repeat contours:";

        for (int i = 0; i < contours.Count; i++)
        {
            for (int j = 0; j < contours.Count; j++)
            {
                if (i != j && contours[i].idElements.Count == contours[j].idElements.Count)
                {
                    if (RepeatContour(contours[i], contours[j]) || RepeatDisplacedContour(contours[i], contours[j]))
                    {
                        //Debug.Log("<color=#22EED4>" + contours[i].nameElements.ShowList("-") + "</color>");

                        text += '\n' + contours[i].nameElements.ShowList("_");
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

    private void MatrixOfContoursOfTheComplex(Contour[] contours, Connection[] connections, int[,] adjacencyMatrix, string[] namesMatrix)
    {
        int[,] matrixArcAndContour = new int[contours.Length, connections.GetLength(0)];
        for (int i = 0; i < matrixArcAndContour.GetLength(0); i++)
        {
            for (int j = 0; j < matrixArcAndContour.GetLength(1); j++)
                matrixArcAndContour[i, j] = ArcInContour(connections[j].nameFrom, connections[j].nameTo, contours[i]) * adjacencyMatrix[connections[j].idFrom, connections[j].idTo];
        }

        (int[] countParametric, int[] countArc) = CalculateCountParametricAndArc(matrixArcAndContour);

        string showMatrix = ShowMatrixOfContoursOfTheComplex(contours, connections, matrixArcAndContour, countParametric, countArc);

        text += '\n' + showMatrix;

        int[,] openAdjacencyMatrix = SplitAdjacencyMatrix(adjacencyMatrix, matrixArcAndContour, countParametric, countArc, connections);
        List<int> path = SAOfOpenedTHS(openAdjacencyMatrix, namesMatrix);
        string result = "";
        result += namesMatrix[path[0] - 1];
        for (int id = 1; id < path.Count; id++)
            result += ", " + namesMatrix[path[id] - 1];

        text += '\n' + "result: ";
        text += '\n' + result;
    }
    private (int[], int[]) CalculateCountParametricAndArc(int[,] matrix)
    {
        int[] countParametric = new int[matrix.GetLength(1)];
        int[] countArc = new int[matrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            countParametric[i] = 0;
            countArc[i] = 0;
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                countParametric[i] = Math.Max(countParametric[i], matrix[j, i]);//параметричность//суммарная параметричность
                if (matrix[j, i] != 0)
                    countArc[i]++;//количество дуг
            }
        }

        return (countParametric, countArc);
    }
    private string ShowMatrixOfContoursOfTheComplex(Contour[] contours, Connection[] connections, int[,] matrixArcAndContour, int[] countP, int[] countArc)
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
    private int[,] SplitAdjacencyMatrix(int[,] adjacencyMatrix, int[,] matrixArcAndContour, int[] countParametric, int[] countArc, Connection[] connections)
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
        int maxCountArc, idTargetArc, minParametric = int.MaxValue;

        while (splitContours.Count > 0)
        {
            maxCountArc = int.MinValue;
            idTargetArc = 0;
            for (int idArc = 0; idArc < countArc.Length; idArc++)
            {
                if (maxCountArc < countArc[idArc] && !usedArcs.Contains(idArc))
                {
                    idTargetArc = idArc;
                    maxCountArc = countArc[idArc];/*countParametric[idArc] - */
                    minParametric = countParametric[idArc];

                }
            }

            for (int idEqualArc = 0; idEqualArc < countArc.Length; idEqualArc++)
            {
                if (countArc[idEqualArc] == maxCountArc && minParametric > countParametric[idEqualArc])
                {
                    minParametric = countParametric[idEqualArc];
                    idTargetArc = idEqualArc;
                }
            }

            usedArcs.Add(idTargetArc);
            int xSplit, ySplit;
            for (int idContour = 0; idContour < matrixArcAndContour.GetLength(0); idContour++)
            {
                if (matrixArcAndContour[idContour, idTargetArc] != 0 && splitContours.Contains(idContour))
                {
                    //Debug.Log($"<color=#AC68FA>Разрываем связь {connections[idTargetArc].name}</color>");
                    text += '\n' + "Cut connection\t" + connections[idTargetArc].name;
                    xSplit = connections[idTargetArc].idFrom;
                    ySplit = connections[idTargetArc].idTo;
                    openAdjacencyMatrix[xSplit, ySplit] = 0;
                    for (int idArc = 0; idArc < matrixArcAndContour.GetLength(1); idArc++)
                        matrixArcAndContour[idContour, idArc] = 0;

                    (countParametric, countArc) = CalculateCountParametricAndArc(matrixArcAndContour);
                    _ = splitContours.Remove(idContour);
                }
            }
        }

        return openAdjacencyMatrix;
    }
    public string StructuralAnalysisChemicalTechnologicalSystems(int[,] adjacencyMatrix)
    {
        if (adjacencyMatrix == null ||
            adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
        {
            return "";
        }

        text = "";
        text += adjacencyMatrix.ShowArray();

        int[,] connectionMatrix = ToConnectionMatrix(adjacencyMatrix);
        text += '\n' + "A = ";
        text += '\n' + connectionMatrix.ShowArray();

        int[,] arrT = connectionMatrix.Transposition();
        text += '\n' + "A^T = ";
        text += '\n' + arrT.ShowArray();
        int[,] complexArr = connectionMatrix.Complex(arrT);
        if (OnlyMainDiagonal(complexArr))
        {
            _ = SAOfOpenedTHS(adjacencyMatrix);
            text += '\n' + "Open system";

            return text;
        }

        string[,] table = complexArr.CreateTable(out string[] names);
        text += '\n' + table.ShowArray();

        int[,] cutAdjacencyMatrix;
        (cutAdjacencyMatrix, _) = CutEmptyColumns(complexArr, adjacencyMatrix, ref names);
        text += '\n' + "cutAdjacencyMatrix:";
        text += '\n' + cutAdjacencyMatrix.CreateTable(names).ShowArray();

        Contour[] contours;
        Connection[] connections;
        (connections, contours) = CreateConnectionsAndContours(cutAdjacencyMatrix, names);
        MatrixOfContoursOfTheComplex(contours, connections, cutAdjacencyMatrix, names);
        text += '\n' + "Close system";
        return text;
    }
    private List<int> SAOfOpenedTHS(int[,] adjacencyMatrix, string[] names)
    {
        if (adjacencyMatrix == null)
            return null;
        int size = adjacencyMatrix.GetLength(0);
        List<int> path = new();
        int id;
        int[] idArr = new int[size];
        for (int i = 0; i < size; i++)
            idArr[i] = i + 1;
        text += '\n' + idArr.ShowArray();
        text += '\n' + adjacencyMatrix.ShowArray();

        for (int i = 0; i < size; i++)
        {

            (id, idArr, adjacencyMatrix) = FindEmptyColumnAndDelete(adjacencyMatrix, idArr);
            text += '\n' + idArr.ShowArray();
            text += '\n' + adjacencyMatrix.ShowArray();

            if (id == -1)
                continue;
            if (idArr.Length != 0)
                path.Add(id);
            if (idArr.Length == 1)
                path.Add(idArr[0]);
        }

        Debug.Log(path.ShowList());
        text += '\n' + "path:";
        text += '\n' + path.ShowList();
        return path;
    }
    private List<int> SAOfOpenedTHS(int[,] adjacencyMatrix)
    {
        if (adjacencyMatrix == null)
            return null;
        int size = adjacencyMatrix.GetLength(0);
        List<int> path = new();
        int id;
        int[] idArr = new int[size];
        for (int i = 0; i < size; i++)
            idArr[i] = i + 1;

        text += '\n' + idArr.ShowArray();
        text += '\n' + adjacencyMatrix.ShowArray();

        for (int i = 0; i < size; i++)
        {

            (id, idArr, adjacencyMatrix) = FindEmptyColumnAndDelete(adjacencyMatrix, idArr);
            text += '\n' + idArr.ShowArray();
            text += '\n' + adjacencyMatrix.ShowArray();

            if (id == -1)
                continue;
            if (idArr.Length != 0)
                path.Add(id);
            if (idArr.Length == 1)
                path.Add(idArr[0]);
        }

        Debug.Log(path.ShowList());
        text += '\n' + "path:";
        text += '\n' + path.ShowList();
        return path;
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