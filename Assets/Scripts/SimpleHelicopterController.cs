using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterController : MonoBehaviour
{
    public Rigidbody rb;

    [Header("UI Buttons")]
    public Button upBtn, downBtn, forwardBtn, backBtn, leftBtn, rightBtn;

    [Header("Settings")]
    public float liftPower = 20f;    // Lift force
    public float movePower = 10f;    // Movement force
    public float tiltAmount = 20f;   // Max tilt angle
    public float tiltSpeed = 2f;     // Tilt smoothing

    // Button states
    private bool upHeld, downHeld, fHeld, bHeld, lHeld, rHeld;

    private Quaternion targetTilt;

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.drag = 1f;
        rb.angularDrag = 2f;

        // Add button listeners
        if (upBtn) AddHoldListener(upBtn, () => upHeld = true, () => upHeld = false);
        if (downBtn) AddHoldListener(downBtn, () => downHeld = true, () => downHeld = false);
        if (forwardBtn) AddHoldListener(forwardBtn, () => fHeld = true, () => fHeld = false);
        if (backBtn) AddHoldListener(backBtn, () => bHeld = true, () => bHeld = false);
        if (leftBtn) AddHoldListener(leftBtn, () => lHeld = true, () => lHeld = false);
        if (rightBtn) AddHoldListener(rightBtn, () => rHeld = true, () => rHeld = false);
    }

    void FixedUpdate()
    {
        // --- Vertical lift ---
        if (upHeld) rb.AddForce(Vector3.up * liftPower, ForceMode.Force);
        if (downHeld) rb.AddForce(Vector3.down * liftPower, ForceMode.Force);

        // --- Movement ---
        Vector3 moveDir = Vector3.zero;

        if (fHeld) moveDir += transform.forward;
        if (bHeld) moveDir -= transform.forward;
        if (rHeld) moveDir += transform.right;
        if (lHeld) moveDir -= transform.right;

        rb.AddForce(moveDir.normalized * movePower, ForceMode.Acceleration);

        // --- Tilt effect (like real helicopter) ---
        float tiltX = (fHeld ? 1 : 0) - (bHeld ? 1 : 0); // Forward/back tilt
        float tiltZ = (rHeld ? 1 : 0) - (lHeld ? 1 : 0); // Left/right tilt

        targetTilt = Quaternion.Euler(tiltX * -tiltAmount, transform.eulerAngles.y, tiltZ * -tiltAmount);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, Time.deltaTime * tiltSpeed);
    }

    // Helper for press+release detection
    void AddHoldListener(Button btn, System.Action onPress, System.Action onRelease)
    {
        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

        var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entryDown.callback.AddListener((_) => onPress());
        trigger.triggers.Add(entryDown);

        var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entryUp.callback.AddListener((_) => onRelease());
        trigger.triggers.Add(entryUp);
    }
}
