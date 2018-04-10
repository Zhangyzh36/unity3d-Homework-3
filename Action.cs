using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Action;

namespace Action
{
    public enum SSActionEventType : int { Started, Completed }

    public interface ISSActionCallback
    {
        void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Completed,
            int intParam = 0, string strParam = null, object objectParam = null);
    }

    public class SSAction : ScriptableObject
    {
        public bool enable = true;
        public bool destroy = false;

        public GameObject gameobject { get; set; }
        public Transform transform { get; set; }
        public ISSActionCallback callback { get; set; }

        protected SSAction() { }

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    public class CCMoveToAction : SSAction
    {
        public Vector3 target;
        public float speed;

        public static CCMoveToAction GetSSAction(Vector3 target, float speed)
        {
            CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
            action.target = target;
            action.speed = speed;
            return action;
        }

        public override void Update()
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
            if (this.transform.position == target)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
        }

        public override void Start()
        {

        }
    }

    public class CCSequenceAction : SSAction, ISSActionCallback
    {
        public List<SSAction> sequence;
        public int repeat = -1;
        public int start = 0;

        public static CCSequenceAction GetSSAction(int repeat, int start, List<SSAction> sequence)
        {
            CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
            action.repeat = repeat;
            action.start = start;
            action.sequence = sequence;
            return action;
        }

        public override void Start()
        {
            foreach (SSAction action in sequence)
            {
                action.gameobject = this.gameobject;
                action.transform = this.transform;
                action.callback = this;
                action.Start();
            }
        }

        public override void Update()
        {
            if (sequence.Count == 0) return;
            if (start < sequence.Count)
            {
                sequence[start].Update();
            }
        }

        public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Completed,
            int intParam = 0, string strParam = null, object objectParam = null)
        {
            source.destroy = false;
            this.start++;
            if (this.start >= sequence.Count)
            {
                this.start = 0;
                if (repeat > 0) repeat--;
                if (repeat == 0)
                {
                    this.destroy = true;
                    this.callback.SSActionEvent(this);
                }
            }
        }

        void OnDestroy()
        {
            foreach (SSAction action in sequence)
            {
                DestroyObject(action);
            }
        }
    }

    public class SSActionManager : MonoBehaviour, ISSActionCallback
    {
        private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
        private List<SSAction> waitingAdd = new List<SSAction>();
        private List<int> waitingDelete = new List<int>();

        protected void Updata()
        {
            foreach (SSAction action in waitingAdd)
                actions[action.GetInstanceID()] = action;
            waitingAdd.Clear();

            foreach (KeyValuePair<int, SSAction> kv in actions)
            {
                SSAction action = kv.Value;
                if (action.destroy)
                {
                    waitingDelete.Add(action.GetInstanceID());
                }
                else if (action.enable)
                {
                    action.Update();
                }
            }

            foreach (int key in waitingDelete)
            {
                SSAction action = actions[key];
                actions.Remove(key);
                DestroyObject(action);
            }
            waitingDelete.Clear();
        }

        public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
        {
            action.gameobject = gameobject;
            action.transform = gameobject.transform;
            action.callback = manager;
            waitingAdd.Add(action);
            action.Start();
        }

        public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Completed,
            int intParam = 0, string strParam = null, object objectParam = null)
        {

        }
    }

    public class CCActionManager : SSActionManager
    {
        public void moveBoat(BoatController boat)
        {
            CCMoveToAction action = CCMoveToAction.GetSSAction(boat.getDestination(), boat.speed);
            this.RunAction(boat.getGameobject(), action, this);
        }

        public void moveCharacter(zyzCharacterController characterController, Vector3 dest)
        {
            Vector3 curPos = characterController.getPosition();
            Vector3 midPos = curPos;
            if (dest.y > curPos.y)
            {
                midPos.y = dest.y;
            }
            else
            {
                midPos.x = dest.x;
            }
            SSAction action1 = CCMoveToAction.GetSSAction(midPos, characterController.speed);
            SSAction action2 = CCMoveToAction.GetSSAction(dest, characterController.speed);
            SSAction seqAction = CCSequenceAction.GetSSAction(1, 0, new List<SSAction> { action1, action2 });
            this.RunAction(characterController.getGameObject(), seqAction, this);
        }
    }
}
