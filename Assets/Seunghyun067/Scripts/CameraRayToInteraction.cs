using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public enum HandObjectState { NON_OBJECT, OBJECT }
public class CameraRayToInteraction : MonoBehaviour
{
    [SerializeField] private GameObject move;
    [SerializeField] private LineRenderer line;
    [SerializeField] public Image imageA;
    [SerializeField] public Image imageX;
    [SerializeField] public Image handImage;
    [SerializeField] public Camera mainCam;
    [SerializeField] public Camera subCam;
    [SerializeField] private Transform uiPoint;

    private HandObjectState curObjectState = HandObjectState.NON_OBJECT;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private LayerMask UIObjectLayer;

    [HideInInspector] public string TargetLayerName;

    private bool isInteracting = false;
    private bool isCloseUping = false;

    private GameObject curUIObject;
    private Item curObject;
    private Item curHoverL;
    private Item curHoverR;
    RaycastHit hitL;
    RaycastHit hitR;
    private void Awake()
    {
        imageA.enabled = false;
        imageX.enabled = false;
        //handImage.enabled = false;
        mainCam.enabled = true;
        subCam.enabled = false;


    }
    private void Update()
    {
        SetPosition();

        RaycastInteraction();
    }
    void SetPosition()
    {
        // subCam.transform.position = mainCam.transform.position;
        // subCam.transform.rotation = Quaternion.Euler(mainCam.transform.rotation.eulerAngles.x, mainCam.transform.rotation.eulerAngles.y, mainCam.transform.rotation.eulerAngles.z);
        // transform.position = mainCam.transform.position;
        // transform.rotation = mainCam.transform.rotation;
        // 
        // Vector3 position = subCam.transform.position;
        // position.y -= 0.5f;
        // Vector3 dir = subCam.transform.forward;
        // dir.y = 0;
        //uiPoint.position = position + dir;
    }

    private void RaycastInteraction()
    {
        if (curObject)
            Debug.Log(curObject.name);

        // ���̴� �׻� ��.
        Physics.Raycast(GimmickManager.Instance.LController.transform.position, GimmickManager.Instance.LController.transform.forward, out hitL, 1);
        Physics.Raycast(GimmickManager.Instance.RController.transform.position, GimmickManager.Instance.RController.transform.forward, out hitR, 1);



        // Ÿ���� ������.
        SetHoverObjectL(hitL);
        SetHoverObjectR(hitR);
        //SetHoverObject(hitR, curHoverR);

        // Ÿ���� ���¿� ���� � �۾��� �Ұǰ� ����.
        InteractionAndCloseUpKeyDown(ControllerType.LEFT, curHoverL);
        InteractionAndCloseUpKeyDown(ControllerType.RIGHT, curHoverR);
        //InteractionAndCloseUpKeyDown();

        // Ÿ�ٰ��� ��ȣ�ۿ�.
        Interaction();

        TestInput();
    }

    void TestInput()
    {

    }

    void SetHoverObjectL(RaycastHit _hit)
    {
        // ��ȣ�ۿ� ���� ���� ���� Ÿ���� �����ش�.  
        // �տ� ������Ʈ�� ��� ���� ���� Ÿ���� �������� �ʴ´�.
        if (isCloseUping || isInteracting || curObjectState == HandObjectState.OBJECT)
            return;

        if (!_hit.transform || _hit.transform.gameObject.layer
            != LayerMask.NameToLayer(GimmickManager.Instance.TargetLayerName))
        {

            curHoverL?.GetItemComponent<Hoverable>().HoverOff();
            curHoverL = null;
            return;
        }

        Item targetObject = null;

        if (!(targetObject = _hit.transform.GetComponent<Item>()))
            return;

        if (!targetObject.IsOption(ItemOption.HOVER))
            return;


        if (targetObject == curHoverL)
            return;

        curHoverL?.GetItemComponent<Hoverable>().HoverOff();
        curHoverL = targetObject;
        curHoverL?.GetItemComponent<Hoverable>().HoverOn();
    }

    void SetHoverObjectR(RaycastHit _hit)
    {
        // ��ȣ�ۿ� ���� ���� ���� Ÿ���� �����ش�.  
        // �տ� ������Ʈ�� ��� ���� ���� Ÿ���� �������� �ʴ´�.
        if (isCloseUping || isInteracting || curObjectState == HandObjectState.OBJECT)
            return;

        if (!_hit.transform || _hit.transform.gameObject.layer
            != LayerMask.NameToLayer(GimmickManager.Instance.TargetLayerName))
        {

            curHoverR?.GetItemComponent<Hoverable>().HoverOff();
            curHoverR = null;
            return;
        }

        Item targetObject = null;

        if (!(targetObject = _hit.transform.GetComponent<Item>()))
            return;

        if (!targetObject.IsOption(ItemOption.HOVER))
            return;


        if (targetObject == curHoverR)
            return;

        curHoverR?.GetItemComponent<Hoverable>().HoverOff();
        curHoverR = targetObject;
        curHoverR?.GetItemComponent<Hoverable>().HoverOn();
    }

    void InteractionAndCloseUpKeyDown(ControllerType type, Item curHover)
    {
        if (isCloseUping)
            return;

        if (isInteracting)
            return;

        // Ÿ���� ���ų� �̹� ��ȣ�ۿ� ���̸� ������ ����.
        if (!(imageA.enabled = curHover))
            return;

        // Ŭ����� �����Ѱ�?
        //imageA.enabled = ;

        if (curHover.IsOption(ItemOption.INTERACTION))
            Debug.Log("Option");

        if (curHover.IsOption(ItemOption.CLOSEUP) == true && (XRInput.Instance.GetKey(ControllerType.LEFT, CommonUsages.primaryButton) || XRInput.Instance.GetKey(ControllerType.RIGHT, CommonUsages.primaryButton)))
        {
            curObject = curHover;
            curObject?.GetItemComponent<Hoverable>().HoverOff();
            curObject.GetItemComponent<CloseUpable>().CloseUp();
            ObjectCreate();
            MainView();
            imageA.enabled = false;
            imageX.enabled = true;
            isCloseUping = true;
            move.SetActive(false);
        }

        // Ű�� ������ ��ȣ�ۿ�
        else if (curHover.IsOption(ItemOption.INTERACTION) && (XRInput.Instance.GetKey(ControllerType.LEFT, CommonUsages.primaryButton) || XRInput.Instance.GetKey(ControllerType.RIGHT, CommonUsages.primaryButton)))
        {
            curObject = curHover;
            Debug.Log("asd");
            curObject?.GetItemComponent<Hoverable>().HoverOff();
            curObject.GetItemComponent<SH.Interactionable>().Interaction();
            isInteracting = true;
        }

        // Ű�� ������ Ŭ�����.
        
    }

    void Interaction()
    {
        // ��ȣ�ۿ� ���� ��..
        if (isInteracting)
        {
            isInteracting = curObject.GetItemComponent<SH.Interactionable>().InteractionUpdate();

            // ���ο��� �����ų� Ű�� ������ ��ȣ�ۿ� ����
            if (!isInteracting || XRInput.Instance.GetKey(ControllerType.LEFT, CommonUsages.secondaryButton) || XRInput.Instance.GetKey(ControllerType.RIGHT, CommonUsages.secondaryButton))
            {
                curObject.GetItemComponent<SH.Interactionable>().UnInteraction();
                InteractionEnd();
            }
        }

        // Ŭ����� ���� ��..
        if (isCloseUping)
        {
            curObject.GetItemComponent<CloseUpable>().CloseUp();

            // XŰ�� ������ Ŭ����� ����.
            if (XRInput.Instance.GetKey(ControllerType.LEFT, CommonUsages.secondaryButton) || XRInput.Instance.GetKey(ControllerType.RIGHT, CommonUsages.secondaryButton))
            {
                curObject.GetItemComponent<CloseUpable>().UnCloseUp();
                InteractionEnd();
                isCloseUping = false;
                move.SetActive(true);
            }
        }
    }

    private void InteractionEnd()
    {

        SubView();
        Destroy(curUIObject);
        curUIObject = null;
        imageX.enabled = false;
        curObject = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 5f);
    }

    public void MainView()
    {
        mainCam.enabled = false;
        subCam.enabled = true;
    }
    public void SubView()
    {
        mainCam.enabled = true;
        subCam.enabled = false;
    }

    void ObjectCreate()
    {
        Vector3 UiPos = uiPoint.position;
        UiPos.x += curObject.GetItemComponent<CloseUpable>().OffsetX;
        UiPos.y += curObject.GetItemComponent<CloseUpable>().OffsetY;

        curUIObject = Instantiate(curObject.transform.gameObject, uiPoint.position, uiPoint.rotation);
        curUIObject.AddComponent<ObjectRotate>();

        if (curUIObject.GetComponent<Rigidbody>())
            curUIObject.GetComponent<Rigidbody>().useGravity = false;
    }
}