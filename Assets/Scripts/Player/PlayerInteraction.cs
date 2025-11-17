using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRange = 1.5f;
    public LayerMask interactableLayer;

    private IInteractable currentTarget;
    private Interactable currentHighlight;

    void Start()
    {
        // Suscribirse a evento del InputManager
        InputManager.Instance.OnInteract += HandleInteract;
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);

        if (hits.Length > 0)
        {
            Collider best = GetClosest(hits);

            IInteractable interactable = best.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Quitar highlight anterior si es distinto
                if (currentHighlight != null && currentHighlight != interactable) currentHighlight.SetHighlight(false);

                // Asignar highlight nuevo
                currentHighlight = interactable as Interactable;
                currentHighlight?.SetHighlight(true);

                currentTarget = interactable;

                Sprite icon = GetInteractIcon();
                string text = icon
                    ? interactable.GetInteractText()
                    : $"[{GetInteractKey()}] {interactable.GetInteractText()}";

                InteractionUI.Instance.Show(text, icon);
                return;
            }
        }

        if (currentHighlight != null)
        {
            currentHighlight.SetHighlight(false);
            currentHighlight = null;
        }

        currentTarget = null;
        InteractionUI.Instance.Hide();
    }

    Collider GetClosest(Collider[] cols)
    {
        Collider best = cols[0];
        float bestDist = Vector3.Distance(transform.position, best.transform.position);

        for (int i = 1; i < cols.Length; i++)
        {
            float d = Vector3.Distance(transform.position, cols[i].transform.position);
            if (d < bestDist)
            {
                best = cols[i];
                bestDist = d;
            }
        }

        return best;
    }

    void HandleInteract()
    {
        currentTarget?.Interact(gameObject);
    }

    string GetInteractKey()
    {
        return InputManager.Instance.GetKeyName("Player/Interact") ?? "E";
    }

    Sprite GetInteractIcon()
    {
        return InputManager.Instance.GetKeyIcon("Player/Interact");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
