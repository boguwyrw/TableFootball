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

    private void Start()
    {
        blueLineNumber = 0;
        numberOfMoves = 1;

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
        PossibleMoves(Ball.ballPositions[Ball.ballPositions.Count - 1]);
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

        RemoveForbiddenPositions(listOfPossibleMoves);
    }

    private void RemoveForbiddenPositions(List<Vector3> positionsList)
    {
        for (int j = 0; j < positionsList.Count; j++)
        {

        }
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
