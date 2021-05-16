using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform goalTarget = null;
    [SerializeField] private Transform ball = null;
    [SerializeField] private List<Transform> doubleMovesOnEdge = new List<Transform>();
    [SerializeField] private GameObject playerRedPrefab = null;
    [SerializeField] private Text winerText = null;

    private LineRenderer redLineRenderer;
    private int redLineNumber;
    private Vector3 startPoint;
    private Vector3 actualPoint;
    private int numberOfMovesUp;
    private int numberOfMovesDown;
    private int numberOfMovesRight;
    private int numberOfMovesLeft;
    private int numberOfMoves;
    private bool canAssignPosition;

    private void Start()
    {
        startPoint = Vector3.zero;
        actualPoint = startPoint;
        redLineNumber = 0;
        numberOfMoves = 1;
        canAssignPosition = true;

        NumbersOfMoves();
        CreatingRedLineRenderer();
    }

    private void Update()
    {
        if (MovementController.playerChanger == 0 && numberOfMoves > 0 && !MovementController.weHaveWiner)
        {
            
            if (canAssignPosition)
            {
                startPoint = ball.position;
                actualPoint = startPoint;
                canAssignPosition = false;
            }
            
            MoveUp();
            MoveDown();
            MoveRight();
            MoveLeft();

            ApprovePosition();
        }

        if (ball.position == new Vector3(0, 6, 0))
        {
            MovementController.weHaveWiner = true;
            winerText.gameObject.SetActive(true);
            winerText.color = Color.red;
            winerText.text = winerText.text + " RED";
        }
    }

    private void NumbersOfMoves()
    {
        numberOfMovesUp = 1;
        numberOfMovesDown = 1;
        numberOfMovesRight = 1;
        numberOfMovesLeft = 1;
    }

    private void MoveUp()
    {
        if (Input.GetKeyDown(KeyCode.W) && numberOfMovesUp != 0)
        {
            actualPoint = new Vector3(actualPoint.x, actualPoint.y + 1, 0);
            redLineRenderer.SetPosition(0, startPoint);
            redLineRenderer.SetPosition(1, actualPoint);
            numberOfMovesUp--;
            numberOfMovesDown++;
        }
    }

    private void MoveDown()
    {
        if (Input.GetKeyDown(KeyCode.S) && numberOfMovesDown != 0)
        {
            actualPoint = new Vector3(actualPoint.x, actualPoint.y - 1, 0);
            redLineRenderer.SetPosition(0, startPoint);
            redLineRenderer.SetPosition(1, actualPoint);
            numberOfMovesDown--;
            numberOfMovesUp++;
        }
    }

    private void MoveRight()
    {
        if (Input.GetKeyDown(KeyCode.D) && numberOfMovesRight != 0)
        {
            actualPoint = new Vector3(actualPoint.x + 1, actualPoint.y, 0);
            redLineRenderer.SetPosition(0, startPoint);
            redLineRenderer.SetPosition(1, actualPoint);
            numberOfMovesRight--;
            numberOfMovesLeft++;
        }
    }

    private void MoveLeft()
    {
        if (Input.GetKeyDown(KeyCode.A) && numberOfMovesLeft != 0)
        {
            actualPoint = new Vector3(actualPoint.x - 1, actualPoint.y, 0);
            redLineRenderer.SetPosition(0, startPoint);
            redLineRenderer.SetPosition(1, actualPoint);
            numberOfMovesLeft--;
            numberOfMovesRight++;
        }
    }

    private void CreatingRedLineRenderer()
    {
        Instantiate(playerRedPrefab, transform);
        redLineRenderer = transform.GetChild(redLineNumber).GetComponent<LineRenderer>();
    }

    private void AdditionalMoveOnEdge()
    {
        for (int i = 0; i < doubleMovesOnEdge.Count; i++)
        {
            if (actualPoint == doubleMovesOnEdge[i].position)
            {
                MovementController.playerChanger = 0;
                numberOfMoves++;
            }
        }
    }


    private void AdditionalMoveInField(Vector3 position)
    {
        for (int j = 0; j < Ball.ballPositions.Count; j++)
        {
            if (position == Ball.ballPositions[j])
            {
                MovementController.playerChanger = 0;
                numberOfMoves++;
            }
        }
    }

    private void ApprovePosition()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovementController.playerChanger = 1;
            AdditionalMoveInField(redLineRenderer.GetPosition(redLineRenderer.positionCount - 1));
            numberOfMoves--;
            ball.position = redLineRenderer.GetPosition(redLineRenderer.positionCount - 1);
            Ball.ballPositions.Add(ball.position);
            AdditionalMoveOnEdge();
            redLineNumber++;
            NumbersOfMoves();
            CreatingRedLineRenderer();
            numberOfMoves = 1;
            canAssignPosition = true;
            AddingAllMoves();
        }
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
