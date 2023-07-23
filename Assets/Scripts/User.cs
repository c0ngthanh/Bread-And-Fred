// using System.Collections;
// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;

// public class User : NetworkBehaviour
// {
//     private Rigidbody2D rb;
//     private BoxCollider2D boxCollider2d;
//     private Animator ani;
//     private SpriteRenderer sprite;
//     [SerializeField] private LayerMask GroundLayer, GroundLayer2;
//     private float jumpSpeed = 18;
//     private float dirX;

//     // rope
//     private HingeJoint2D hj;

//     public float pushForce = 10f;
//     public bool attached = false;
//     public Transform attachedTo;
//     // private GameObject disregard;

//     // public GameObject pulleySelected = null;
//     // Vector2 movement;
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         ani = GetComponent<Animator>();
//         sprite = GetComponent<SpriteRenderer>();
//         boxCollider2d = GetComponent<BoxCollider2D>();
//     }

//     void Awake()
//     {
//         hj = gameObject.GetComponent<HingeJoint2D>();

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // Slide(-1);

//         dirX = Input.GetAxisRaw("Horizontal");
//         CheckKeyboardInputs(dirX);
//     }


//     private void CheckKeyboardInputs(float dirX)
//     {
//         {
//             if (Input.GetKeyDown(KeyCode.W) && isGrounded())
//             {
//                 GetComponent<Rigidbody2D>().velocity = new Vector2(dirX, jumpSpeed * 1f);
//             }
//             if (Input.GetKey(KeyCode.S))
//             {
//                 if (isGrounded())
//                 {
//                     ani.SetBool("sitting", true);
//                     GetComponent<Rigidbody2D>().velocity = new Vector2(dirX * 0f, 0);
//                 }

//             }
//             else
//             {
//                 ani.SetBool("sitting", false);
//                 rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
//                 updateAnimation(dirX);
//             }
//         }

//         // else
//         // {
//         //     if (Input.GetKey("left") || Input.GetKey("a"))
//         //     {
//         //         if (attached)
//         //         {
//         //             rb.AddRelativeForce(new Vector3(-1, 0, 0) * pushForce);
//         //         }
//         //     }
//         //     if (Input.GetKey("right") || Input.GetKey("d"))
//         //     {
//         //         if (attached)
//         //         {
//         //             rb.AddRelativeForce(new Vector3(1, 0, 0) * pushForce);

//         //         }
//         //     }
//         // }

//         if (Input.GetKeyDown("up") && attached)
//         {
//             Slide(1);
//         }
//         if (Input.GetKeyDown("down") && attached)
//         {
//             Slide(-1);
//         }
//         if (Input.GetKey("left") || Input.GetKey("a"))
//         {
//             if (attached)
//             {
//                 rb.AddRelativeForce(new Vector3(-1, 0, 0) * pushForce);
//             }
//         }
//         if (Input.GetKey("right") || Input.GetKey("d"))
//         {
//             if (attached)
//             {
//                 rb.AddRelativeForce(new Vector3(1, 0, 0) * pushForce);

//             }
//         }
//         if (Input.GetKeyDown("w") && isGrounded())
//         {
//             if (attached)
//             {
//                 rb.velocity = new Vector2(rb.velocity.x, pushForce);
//                 transform.Translate(new Vector3(0, pushForce, 0) * Time.deltaTime);

//             }
//         }
//         // if (Input.GetKeyDown(KeyCode.Space))
//         // {
//         //     Detach();
//         // }
//     }

//     // private void playerMove(float dirX)
//     // {
//     //     if (Input.GetKey(KeyCode.S))
//     //     {
//     //         if (isGrounded())
//     //         {
//     //             ani.SetBool("sitting", true);
//     //             GetComponent<Rigidbody2D>().velocity = new Vector2(dirX * 0f, 0);
//     //         }
//     //     }
//     //     else
//     //     {
//     //         ani.SetBool("sitting", false);
//     //         rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
//     //         updateAnimation(dirX);
//     //     }
//     //     if (Input.GetKey("left"))
//     //     {
//     //         if (attached)
//     //         {
//     //             rb.AddRelativeForce(new Vector3(-1, 0, 0) * pushForce);
//     //         }
//     //     }
//     //     if (Input.GetKey("right"))
//     //     {
//     //         if (attached)
//     //         {
//     //             rb.AddRelativeForce(new Vector3(1, 0, 0) * pushForce);

//     //         }
//     //     }
//     // }

//     private void updateAnimation(float dirX)
//     {
//         if (dirX > 0f)
//         {
//             ani.SetBool("running", true);
//             sprite.flipX = false;
//         }
//         else if (dirX < 0f)
//         {
//             ani.SetBool("running", true);
//             sprite.flipX = true;
//         }
//         else
//         {
//             ani.SetBool("running", false);
//         }

//     }

//     private bool isGrounded()
//     {
//         // return transform.Find("GroundCheck").GetComponent<GroundCheck>().isGrounded;
//         float extraHeightText = .2f;
//         RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, GroundLayer);
//         RaycastHit2D raycastHit2 = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, GroundLayer2);
//         Color rayColor;
//         if (raycastHit.collider != null || raycastHit2.collider != null)
//         {
//             rayColor = Color.green;
//         }
//         else
//         {
//             rayColor = Color.red;
//         }
//         Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
//         Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
//         Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
//         // Debug.Log(raycastHit.collider);
//         return raycastHit.collider != null || raycastHit2.collider != null;

//     }

//     /* 
//     left = swing left on rope
//     right arrow = swing right on rope
//     spac = detach from rope
//     up arrow = climb up rope
//     down = climb down rope
//     */


//     // void CheckKeyboardInputs()
//     // {
//     //     if (Input.GetKey("left"))
//     //     {
//     //         if (attached)
//     //         {
//     //             rb.AddRelativeForce(new Vector3(-1, 0, 0) * pushForce);
//     //         }
//     //     }
//     //     if (Input.GetKey("right"))
//     //     {
//     //         if (attached)
//     //         {
//     //             rb.AddRelativeForce(new Vector3(1, 0, 0) * pushForce);

//     //         }
//     //     }
//     //     if (Input.GetKeyDown("up") && attached)
//     //     {
//     //         Slide(1);
//     //     }
//     //     if (Input.GetKeyDown("down") && attached)
//     //     {
//     //         Slide(-1);
//     //     }
//     //     if (Input.GetKeyDown(KeyCode.Space))
//     //     {
//     //         Detach();
//     //     }
//     // }

//     // dính lên dây
//     public void Attach(Rigidbody2D ropeBone)
//     {
//         // ropeBone.gameObject.GetComponent<RopeSegment>().isPlayerAttached = true;
//         hj.connectedBody = ropeBone;
//         hj.enabled = true;
//         attached = true;
//         attachedTo = ropeBone.gameObject.transform.parent;

//         // Update();

//     }

//     // rời khỏi dây
//     // void Detach()
//     // {
//     //     hj.connectedBody.gameObject.GetComponent<RopeSegment>().isPlayerAttached = false;
//     //     attached = false;
//     //     hj.enabled = false;
//     //     hj.connectedBody = null;
//     // }

//     // lên xuống dây
//     public void Slide(int direction)
//     {
//         // RopeSegment myConnection = hj.connectedBody.gameObject.GetComponent<RopeSegment>();
//         GameObject newSeg = null;
//         if (direction > 0)
//         {
//             if (myConnection.connectedAbove != null)
//             {
//                 if (myConnection.connectedAbove.gameObject.GetComponent<RopeSegment>() != null)
//                 {
//                     newSeg = myConnection.connectedAbove;
//                 }
//             }
//         }
//         else
//         {
//             if (myConnection.connectedBelow != null)
//             {
//                 newSeg = myConnection.connectedBelow;
//             }
//         }
//         if (newSeg != null)
//         {
//             transform.position = newSeg.transform.position;
//             myConnection.isPlayerAttached = false;
//             // newSeg.GetComponent<RopeSegment>().isPlayerAttached = true;
//             hj.connectedBody = newSeg.GetComponent<Rigidbody2D>();
//         }
//     }

//     void OnTriggerEnter2D(Collider2D col)
//     {
//         if (!attached)
//         {
//             if (col.gameObject.tag == "Rope")
//             {
//                 if (attachedTo != col.gameObject.transform.parent)
//                 {
//                     {
//                         Attach(col.gameObject.GetComponent<Rigidbody2D>());
//                     }
//                 }
//             }
//         }
//     }


//     // void CheckPulleyInputs()
//     // {
//     //     if (Input.GetMouseButtonDown(0))
//     //     {
//     //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//     //         Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
//     //         RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
//     //         if (hit.collider != null && hit.transform.gameObject.tag == "Crank")
//     //         {
//     //             if (pulleySelected != hit.transform.gameObject)
//     //             {
//     //                 if (pulleySelected != null)
//     //                 {
//     //                     pulleySelected.GetComponent<crank>().Deselect();
//     //                 }

//     //                 pulleySelected = hit.transform.gameObject;
//     //                 pulleySelected.GetComponent<crank>().Select();
//     //             }
//     //             else if (pulleySelected == hit.transform.gameObject)
//     //             {
//     //                 pulleySelected.GetComponent<crank>().Deselect();
//     //                 pulleySelected = null;
//     //             }
//     //         }
//     //         else
//     //         {
//     //             if (pulleySelected != null)
//     //             {
//     //                 pulleySelected.GetComponent<crank>().Deselect();
//     //                 pulleySelected = null;
//     //             }

//     //         }
//     //     }
//     //     if (Input.GetKeyDown("f") && pulleySelected != null)
//     //     {
//     //         pulleySelected.GetComponent<crank>().Rotate(1);
//     //     }
//     //     if (Input.GetKeyDown("r") && pulleySelected != null)
//     //     {
//     //         pulleySelected.GetComponent<crank>().Rotate(-1);
//     //     }
//     // }
// }