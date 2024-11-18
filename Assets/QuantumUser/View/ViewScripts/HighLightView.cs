namespace Quantum
{
    using UnityEngine;

    public class HighLightView : QuantumEntityViewComponent
    {
        //[SerializeField] private GameObject taskPanel;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            QuantumEvent.Subscribe(listener: this, handler: (EventIsHighLight e) => HighLight(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventInitiatingTask e) => InitiatingTask(e));
            spriteRenderer.enabled = false;
        }

        private void InitiatingTask(EventInitiatingTask e)
        {

        }

        private void HighLight(EventIsHighLight e)
        {
            if (QuantumRunner.DefaultGame.GetLocalPlayers()[0] != e.PlayerRef || _entityView.EntityRef != e.TaskRef)
            {
                return;
            }
            spriteRenderer.enabled = e.IsEnter;
            //taskPanel.SetActive(e.IsEnter);
        }

        private void Update()
        {

        }
    }
}
