using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraft : MonoBehaviour
{
    public float health;
    public float speed;
    public float baseSpeed;
    public int score;
    public int currentLine;
    public enum Direction { east, west}
    public Direction direction;
    public bool move;
    public bool letMove;
    public Transform leftPoint;
    public Transform rightPoint;
    public bool bombing;
    public ManageAirCraft manageAirCraft;
    public LayerMask layerMask;
    public GameObject bomb;
    public GameObject bombPoint;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        manageAirCraft = GameObject.FindObjectOfType<ManageAirCraft>();
    }

    // Update is called once per frame
    void Update()
    {
        if (letMove)
        {
            if (!manageAirCraft.Lines[currentLine])
            {
                letMove = false;
                if (bombing)
                {
                    if (manageAirCraft.AntiAirRaidCenter.activeSelf)
                    {
                        target = manageAirCraft.AntiAirRaidCenter;
                        transform.position = new
                            Vector3(transform.position.x, transform.position.y, manageAirCraft.AntiAirRaidCenter.transform.position.z);
                    }
                    else
                    {
                        for (int i = 0; i < manageAirCraft.targets.Length; i++)
                        {
                            if (manageAirCraft.targets[i].TryGetComponent(out TargetBuilding targetBuilding))
                            {
                                if (!targetBuilding.InUse)
                                {
                                    if (target == null) target = manageAirCraft.targets[i];
                                    if (Mathf.Abs(transform.position.x - manageAirCraft.targets[i].transform.position.x) <
                                        Mathf.Abs(transform.position.x - target.transform.position.x))
                                    {
                                        target = manageAirCraft.targets[i];

                                    }
                                }
                            }
                        }
                        if (target)
                        {
                            transform.position = new 
                                Vector3(transform.position.x, transform.position.y, target.transform.position.z);
                        }
                    }
                }
                move = true;
            }
        }
        if (move)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            manageAirCraft.Lines[currentLine] = true;
            switch (direction)
            {
                case Direction.east:
                    if (transform.position.x > leftPoint.transform.position.x)
                    {
                        ChangeLine();
                    }
                    break;
                case Direction.west:
                    if (transform.position.x < rightPoint.transform.position.x)
                    {
                        ChangeLine();
                    }
                    break;
            }
        }
        if (bombing)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "target")
                {
                    if (hit.collider.gameObject.TryGetComponent(out TargetBuilding targetBuilding))
                    {
                        if (!targetBuilding.InUse)
                        {
                            GameObject cloneAirCraft = Instantiate(bomb, bombPoint.transform.position, bombPoint.transform.rotation);
                            cloneAirCraft.GetComponent<Rigidbody>().AddForce(Vector3.up * -500);
                            bombing = false;
                            targetBuilding.InUse = true;
                        }
                    }
                }
            }
        }
    }

    public void ChangeLine()
    {
        //Debug.Log("ChangeLine");
        move = false;
        letMove = false;
        manageAirCraft.Lines[currentLine] = false;
        currentLine++;
        if (currentLine > manageAirCraft.Lines.Length-1)/// delete object
        {
            Destroy(gameObject);
        }
        else
        {
            //manageAirCraft.Lines[currentLine] = true;
            if (currentLine == manageAirCraft.Lines.Length - 1)///bombing
            {
                bombing = true;

            }
            if (direction == Direction.west) direction = Direction.east;
            else direction = Direction.west;
            transform.position = new Vector3(transform.position.x, transform.position.y - manageAirCraft.heightLines, transform.position.z);
            transform.rotation = Quaternion.Inverse(transform.rotation);
            letMove = true;
        }
    }
}
