using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interactable")]
    [Tooltip("Texto que aparece en la UI.")]
    public string interactText = "Interactuar";

    [Tooltip("Si existe un MeshRenderer, se usa para resaltar.")]
    public bool enableHighlight = true;
    public Color highlightColor = Color.yellow;

    private MeshRenderer[] meshRenderers;
    private Color[] originalColors;
    private bool isHighlighted = false;

    protected virtual void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        if (meshRenderers.Length > 0)
        {
            originalColors = new Color[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
                originalColors[i] = meshRenderers[i].material.color;
        }
    }

    public virtual string GetInteractText()
    {
        return interactText;
    }

    public abstract void Interact(GameObject interactor);

    public void SetHighlight(bool active)
    {
        if (!enableHighlight || meshRenderers.Length == 0) return;
        if (isHighlighted == active) return;

        isHighlighted = active;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = active ? highlightColor : originalColors[i];
        }
    }
}
