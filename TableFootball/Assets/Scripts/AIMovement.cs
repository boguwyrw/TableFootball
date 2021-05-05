using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private Transform goalTarget = null;
    [SerializeField] private Transform ball = null;
    [SerializeField] private List<Transform> doubleMovesOnEdge = new List<Transform>();
    [SerializeField] private GameObject AIBluePrefab = null;

    private LineRenderer blueLineRenderer;
    private int blueLineNumber;
    private int numberOfMoves;
    private Vector3 AISelectedPosition;
    private List<Vector3> listOfPossibleMoves = new List<Vector3>();
    private Vector3 AIActualPosition;

    private void Start()
    {
        blueLineNumber = 0;
        numberOfMoves = 1;
        AIActualPosition = Vector3.zero;

        CreatingBlueLineRenderer();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && MovementController.playerChanger == 1 && numberOfMoves > 0 && !MovementController.weHaveWiner)
        {
            StartCoroutine(DelayAI_Action());
        }

        if (ball.position == new Vector3(0, -6, 0))
            MovementController.weHaveWiner = true;
    }

    private void CreatingBlueLineRenderer()
    {
        Instantiate(AIBluePrefab, transform);
        blueLineRenderer = transform.GetChild(blueLineNumber).GetComponent<LineRenderer>();
    }

    private IEnumerator DelayAI_Action()
    {
        yield return new WaitForSeconds(0.6f);
        PossibleMoves(Ball.ballPositions[Ball.ballPositions.Count - 1]); // ETAP 1
        RemoveForbiddenPositions(listOfPossibleMoves); // ETAP 2
        RemoveEdgePositions(); // ETAP 3
        RemovePositionFartherFromGoal(); // ETAP 4
        CheckNextPossibleMoves(); // ETAP 5
        blueLineRenderer.SetPosition(0, Ball.ballPositions[Ball.ballPositions.Count - 1]);
        blueLineRenderer.SetPosition(1, SelectedPosition());
        ApprovePosition();
    }

    private void PossibleMoves(Vector3 actualPosition)
    {
        Vector3 forbiddenPosition = Ball.ballPositions[Ball.ballPositions.Count - 2];
        List<Vector3> tempPossibleMovesList = new List<Vector3>();
        // liczenie możliwych ruchów zaczyna się od godziny 3 i idzie w przeciwnym kierunku do ruchu wskazówek zegara
        tempPossibleMovesList.Add(new Vector3(actualPosition.x + 1, actualPosition.y, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x + 1, actualPosition.y + 1, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x, actualPosition.y + 1, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x - 1, actualPosition.y + 1, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x - 1, actualPosition.y, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x - 1, actualPosition.y - 1, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x, actualPosition.y - 1, 0));
        tempPossibleMovesList.Add(new Vector3(actualPosition.x + 1, actualPosition.y - 1, 0));

        listOfPossibleMoves.Clear();

        for (int i = 0; i < tempPossibleMovesList.Count; i++)
        {
            if (tempPossibleMovesList[i] != forbiddenPosition)
            {
                listOfPossibleMoves.Add(tempPossibleMovesList[i]);
            }
        }
        //Debug.Log("Etap 1: " + listOfPossibleMoves.Count);
        AIActualPosition = actualPosition;
    }

    private void RemoveForbiddenPositions(List<Vector3> positionsList)
    {
        foreach (List<Vector3> positionIn in MovementController.positionsInOut.ToArray())
        {
            if (positionIn[0] == AIActualPosition)
            {
                foreach (Vector3 position in positionsList.ToArray())
                {
                    if (position == positionIn[1])
                        listOfPossibleMoves.Remove(position);
                }
            }
        }
        //Debug.Log("Etap 2: " + listOfPossibleMoves.Count);
    }

    private void RemoveEdgePositions()
    {
        if(AIActualPosition.x == -4)
        {
            foreach (Vector3 itemPos in listOfPossibleMoves.ToArray())
            {
                if (itemPos.x <= -4)
                    listOfPossibleMoves.Remove(itemPos);
            }
        }
        else if(AIActualPosition.x == 4)
        {
            foreach (Vector3 itemPos in listOfPossibleMoves.ToArray())
            {
                if (itemPos.x >= 4)
                    listOfPossibleMoves.Remove(itemPos);
            }
        }

        if (AIActualPosition.y == -5 && AIActualPosition.x < -1 || AIActualPosition.x > 1)
        {
            foreach (Vector3 itemPos in listOfPossibleMoves.ToArray())
            {
                if (itemPos.y <= -5)
                    listOfPossibleMoves.Remove(itemPos);
            }
        }
        else if(AIActualPosition.y == 5 && AIActualPosition.x < -1 || AIActualPosition.x > 1)
        {
            foreach (Vector3 itemPos in listOfPossibleMoves.ToArray())
            {
                if (itemPos.y >= 5)
                    listOfPossibleMoves.Remove(itemPos);
            }
        }
        //Debug.Log("Etap 3: " + listOfPossibleMoves.Count);
    }

    private void CheckNextPossibleMoves()
    {
        List<List<Vector3>> nextOfPossibleMoves = new List<List<Vector3>>();
        for (int w = 0; w < listOfPossibleMoves.Count; w++)
        {
            nextOfPossibleMoves.Add(new List<Vector3>());
        }
        // ETAP 5 - 1
        for (int k = 0; k < listOfPossibleMoves.Count; k++)
        {
            Vector3 nextForbiddenPosition = Ball.ballPositions[Ball.ballPositions.Count - 1];
            List<Vector3> tempNextPossibleMoves = new List<Vector3>();
            // liczenie możliwych ruchów zaczyna się od godziny 3 i idzie w przeciwnym kierunku do ruchu wskazówek zegara
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x + 1, listOfPossibleMoves[k].y, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x + 1, listOfPossibleMoves[k].y + 1, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x, listOfPossibleMoves[k].y + 1, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x - 1, listOfPossibleMoves[k].y + 1, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x - 1, listOfPossibleMoves[k].y, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x - 1, listOfPossibleMoves[k].y - 1, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x, listOfPossibleMoves[k].y - 1, 0));
            tempNextPossibleMoves.Add(new Vector3(listOfPossibleMoves[k].x + 1, listOfPossibleMoves[k].y - 1, 0));

            nextOfPossibleMoves[k] = tempNextPossibleMoves;

            foreach (Vector3 nextPossibleMove in nextOfPossibleMoves[k].ToArray())
            {
                if (nextPossibleMove == nextForbiddenPosition)
                    nextOfPossibleMoves[k].Remove(nextPossibleMove);
            }
        }

        // ETAP 5 - 2
        for (int a = 0; a < listOfPossibleMoves.Count; a++)
        {
            foreach (List<Vector3> posInOut in MovementController.positionsInOut.ToArray())
            {
                if (posInOut[0] == listOfPossibleMoves[a])
                {
                    foreach (Vector3 position in nextOfPossibleMoves[a].ToArray())
                    {
                        if (position == posInOut[1])
                            nextOfPossibleMoves[a].Remove(position);
                    }
                }
            }
        }

        // ETAP 5 - 3
        for (int i = 0; i < nextOfPossibleMoves.Count; i++)
        {
            if (listOfPossibleMoves[i].x == -4)
            {
                foreach (Vector3 itemPos in nextOfPossibleMoves[i].ToArray())
                {
                    if (itemPos.x <= -4)
                        nextOfPossibleMoves[i].Remove(itemPos);
                }
            }
            else if (listOfPossibleMoves[i].x == 4)
            {
                foreach (Vector3 itemPos in nextOfPossibleMoves[i].ToArray())
                {
                    if (itemPos.x >= 4)
                        nextOfPossibleMoves[i].Remove(itemPos);
                }
            }

            if (listOfPossibleMoves[i].y == -5 && listOfPossibleMoves[i].x < -1 || listOfPossibleMoves[i].x > 1)
            {
                foreach (Vector3 itemPos in nextOfPossibleMoves[i].ToArray())
                {
                    if (itemPos.y <= -5)
                        nextOfPossibleMoves[i].Remove(itemPos);
                }
            }
            else if (listOfPossibleMoves[i].y == 5 && listOfPossibleMoves[i].x < -1 || listOfPossibleMoves[i].x > 1)
            {
                foreach (Vector3 itemPos in nextOfPossibleMoves[i].ToArray())
                {
                    if (itemPos.y >= 5)
                        nextOfPossibleMoves[i].Remove(itemPos);
                }
            }
        }

        for (int b = 0; b < nextOfPossibleMoves.Count; b++)
        {
            //Debug.Log(b + " - nextOfPossibleMoves: " + nextOfPossibleMoves[b].Count);
            if(nextOfPossibleMoves[b].Count == 0)
            {
                listOfPossibleMoves.RemoveAt(b);
                nextOfPossibleMoves.RemoveAt(b);
            }
        }
        
        /*
        foreach (List<Vector3> item in nextOfPossibleMoves.ToArray())
        {
            if(item.Count == 0)
            {
                nextOfPossibleMoves.Remove(item);
                listOfPossibleMoves.Remove(item.FindLastIndex(item.Count == 0));
            }
        }
        */
        Debug.Log("Etap 5: " + listOfPossibleMoves.Count);
    }

    private void RemovePositionFartherFromGoal()
    {
        List<Vector3> tempListOfMoves = new List<Vector3>();
        bool canMove;
        for (int i = 0; i < listOfPossibleMoves.Count; i++)
        {
            canMove = (-6 - listOfPossibleMoves[i].y) >= (-6 - AIActualPosition.y);
            if (canMove)
            {
                tempListOfMoves.Add(listOfPossibleMoves[i]);
            }
        }

        listOfPossibleMoves.Clear();
        listOfPossibleMoves = tempListOfMoves;

        Debug.Log("Etap 4: " + listOfPossibleMoves.Count);
    }

    private Vector3 SelectedPosition()
    {
        AISelectedPosition = listOfPossibleMoves[Random.Range(0, listOfPossibleMoves.Count)];
        return AISelectedPosition;
    }

    private void AdditionalMoveOnEdge()
    {
        for (int i = 0; i < doubleMovesOnEdge.Count; i++)
        {
            if (AISelectedPosition == doubleMovesOnEdge[i].position)
            {
                MovementController.playerChanger = 1;
                numberOfMoves++;
            }
        }
    }

    private void AdditionalMoveInField(Vector3 position)
    {
        for (int j = 0; j < Ball.ballPositions.Count; j++)
        {
            if (AISelectedPosition == Ball.ballPositions[j])
            {
                MovementController.playerChanger = 1;
                numberOfMoves++;
            }
        }
    }

    private void ApprovePosition()
    {
        MovementController.playerChanger = 0;
        AdditionalMoveInField(blueLineRenderer.GetPosition(blueLineRenderer.positionCount - 1));
        numberOfMoves--;
        ball.position = blueLineRenderer.GetPosition(blueLineRenderer.positionCount - 1);
        Ball.ballPositions.Add(ball.position);
        AdditionalMoveOnEdge();
        blueLineNumber++;
        numberOfMoves = 1;
        CreatingBlueLineRenderer();
        AddingAllMoves();
    }

    private void AddingAllMoves()
    {
        if (Ball.ballPositions.Count > 1)
        {
            List<Vector3> tempPositionIn = new List<Vector3>();
            tempPositionIn.Add(Ball.ballPositions[Ball.ballPositions.Count - 2]); // wejscie
            tempPositionIn.Add(Ball.ballPositions[Ball.ballPositions.Count - 1]); // wyjscie

            MovementController.positionsInOut.Add(tempPositionIn);

            List<Vector3> tempPositionOut = new List<Vector3>();
            tempPositionOut.Add(Ball.ballPositions[Ball.ballPositions.Count - 1]); // wejscie
            tempPositionOut.Add(Ball.ballPositions[Ball.ballPositions.Count - 2]); // wyjscie

            MovementController.positionsInOut.Add(tempPositionOut);
        }
    }
}
