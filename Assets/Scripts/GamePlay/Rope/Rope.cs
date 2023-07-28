using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rope : NetworkBehaviour
{
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject ropeMax;
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public float ropeSegLen = 0.25f;
    public int segmentLength;
    private float lineWidth = 0.1f;
    private SpringJoint2D joint;

    public float moveSpeed;
    public Vector3 movement;
    public float dirX;
    private LineRenderer ropeMaxLinerenderer;
    // Use this for initialization
    void Start()
    {
        if (IsServer)
        {
            SetPlayerClientRpc();
        }
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = new Vector3(this.player1.transform.position.x, this.player1.transform.position.y, -0.1f);

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
        joint = player1.GetComponent<SpringJoint2D>();
        joint.connectedBody = player2.GetComponent<Rigidbody2D>();
        ropeMaxLinerenderer = ropeMax.GetComponent<LineRenderer>();
        ropeMaxLinerenderer.enabled = false;
    }
    [ClientRpc]
    private void SetPlayerClientRpc()
    {
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        player1 = gameObjectArray[0];
        if (gameObjectArray.Length > 1)
        {
            player2 = gameObjectArray[1];
        }
        else
        {
            player2 = gameObjectArray[0];
        }
    }
    private void Update()
    {
        this.DrawRope();
        float distance = Vector2.Distance(player1.transform.position, player2.transform.position);
        if (Mathf.Sign(player2.transform.position.x - player1.transform.position.x) == player1.GetComponent<PlayerController>().GetSignFacingRight() &&
        joint.enabled &&
        (player1.GetComponent<PlayerController>().IsGrounded() &&
        player2.GetComponent<PlayerController>().IsGrounded()))
        {
            joint.enabled = false;
            ropeMaxLinerenderer.enabled = false;
            lineRenderer.enabled = true;
        }
        else if (Mathf.Sign(player1.transform.position.x - player2.transform.position.x) == player2.GetComponent<PlayerController>().GetSignFacingRight() &&
        joint.enabled &&
        (player1.GetComponent<PlayerController>().IsGrounded() &&
        player2.GetComponent<PlayerController>().IsGrounded()))
        {
            joint.enabled = false;
            ropeMaxLinerenderer.enabled = false;
            lineRenderer.enabled = true;
        }
        if (distance > 4 && !joint.enabled)
        {
            joint.enabled = true;
            ropeMaxLinerenderer.enabled = true;
            lineRenderer.enabled = false;
        }

        // if ((player1.GetComponent<PlayerController>().GetPlayerState() == PlayerController.PlayerState.Sitting ||
        //      player2.GetComponent<PlayerController>().GetPlayerState() == PlayerController.PlayerState.Sitting) &&
        //      GameState.GetGameState() == GameState.State.Normal)
        // {
        //     if (IsServer)
        //     {
        //         SetGameStateServerRpc(GameState.State.Rotate);
        //     }
        // }
        // else if (player1.GetComponent<PlayerController>().GetPlayerState() != PlayerController.PlayerState.Sitting &&
        // player2.GetComponent<PlayerController>().GetPlayerState() != PlayerController.PlayerState.Sitting &&
        // GameState.GetGameState() == GameState.State.Rotate)
        {
            if (IsServer)
            {
                SetGameStateServerRpc(GameState.State.Normal);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetGameStateServerRpc(GameState.State value)
    {
        SetGameStateClientRpc(value);
    }
    [ClientRpc]
    public void SetGameStateClientRpc(GameState.State value)
    {
        GameState.SetGameState(value);
    }
    public void SetPlayer1(GameObject player1)
    {
        this.player1 = player1;
    }
    public void SetPlayer2(GameObject player2)
    {
        this.player2 = player2;
    }
    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector3 forceGravity = new Vector3(0f, -1.5f, 0);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        // for (int i = 0; i < 50; i++)
        // {
        this.ApplyConstraint();
        // }
    }

    private void ApplyConstraint()
    {
        //Constrant to Mouse
        RopeSegment firstSegment = this.ropeSegments[0];
        // firstSegment.posNow = Vector3.zero;
        firstSegment.posNow = new Vector3(this.player1.transform.position.x, this.player1.transform.position.y, -0.1f);
        this.ropeSegments[0] = firstSegment;
        //Constrant to Second Point 
        RopeSegment endSegment = this.ropeSegments[this.ropeSegments.Count - 1];
        endSegment.posNow = new Vector3(this.player2.transform.position.x, this.player2.transform.position.y, -0.1f);
        this.ropeSegments[this.ropeSegments.Count - 1] = endSegment;
        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector3 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);


        //Max rope length
        Vector3[] ropeMaxPositions = new Vector3[2];
        ropeMaxPositions[0] = new Vector3(player1.transform.position.x, player1.transform.position.y, -0.1f);
        ropeMaxPositions[1] = new Vector3(player2.transform.position.x, player2.transform.position.y, -0.1f);
        ropeMaxLinerenderer.startWidth = lineWidth;
        ropeMaxLinerenderer.endWidth = lineWidth;
        ropeMaxLinerenderer.positionCount = 2;
        ropeMaxLinerenderer.SetPositions(ropeMaxPositions);
    }

    public struct RopeSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}