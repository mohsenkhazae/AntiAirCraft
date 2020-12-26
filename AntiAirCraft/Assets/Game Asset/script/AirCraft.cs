using System;
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
    public GameObject prop;
    private Collider _collider;
    public enum StateMove { MOVING, WAITING }
    public StateMove state = StateMove.WAITING;
    RaycastHit hit;
    private float searchCountDown = .5f;

    // Start is called before the first frame update
    void Start()
    {
        //manageAirCraft = GameObject.FindObjectOfType<ManageAirCraft>();
        _collider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == StateMove.WAITING)
        {
            if (LineIsEmpty())
            {
                if (bombing)
                {
                    SetPosition();
                }
                manageAirCraft.Lines[currentLine] = true;
                state = StateMove.MOVING;
            }
            else return;
        }
        if (state == StateMove.MOVING)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            if (EndLine())
            {
                ChangeLine();
            }
        }
        if (bombing)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "target")
                {
                    if (hit.collider.gameObject.TryGetComponent(out TargetBuilding targetBuilding))
                    {
                        if (!targetBuilding.InUse)
                        {
                            GameObject cloneBomb = Instantiate(bomb, bombPoint.transform.position, bombPoint.transform.rotation);
                            cloneBomb.GetComponent<Rigidbody>().AddForce(Vector3.up * -1000);
                            cloneBomb.GetComponent<BombExplosion>().manageAirCraft = manageAirCraft;
                            targetBuilding.InUse = true;
                            bombing = false;
                        }
                    }
                }
            }
        }
        if (prop)
        {
            prop.transform.Rotate(Vector3.forward * Time.deltaTime * 1000);
        }
    }

    private void SetPosition()
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
        _collider.enabled = false;
    }

    bool LineIsEmpty()
    {
        if (!manageAirCraft.Lines[currentLine])
        {
            return true;
        }
        return false;
    }
    bool EndLine()
    {
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0f)
        {
            searchCountDown = .5f;
            switch (direction)
            {
                case Direction.east:
                    if (transform.position.x > leftPoint.transform.position.x)
                    {
                        return true;
                    }
                    break;
                case Direction.west:
                    if (transform.position.x < rightPoint.transform.position.x)
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }
    public void ChangeLine()
    {
        //Debug.Log("ChangeLine");
        manageAirCraft.Lines[currentLine] = false;
        currentLine++;
        if (currentLine > manageAirCraft.Lines.Length-1)/// delete object
        {
            Destroy(gameObject);
        }
        else
        {
            if (currentLine == manageAirCraft.Lines.Length - 1)///bombing
            {
                bombing = true;

            }
            if (direction == Direction.west) direction = Direction.east;
            else direction = Direction.west;
            transform.position = new Vector3(transform.position.x, transform.position.y - manageAirCraft.heightLines, transform.position.z);
            transform.rotation = Quaternion.Inverse(transform.rotation);
            state = StateMove.WAITING;
        }
    }
}
