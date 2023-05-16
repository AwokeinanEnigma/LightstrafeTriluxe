using System;
using CommandUndoRedo;
using UnityEngine;
using System.Collections.Generic;
using Level_Editor_Scripts;
using UnityEditor;

namespace RuntimeGizmos
{
	public abstract class SelectCommand : ICommand
	{
		protected Transform target;
		protected TransformGizmo transformGizmo;

		public SelectCommand(TransformGizmo transformGizmo, Transform target)
		{
			this.transformGizmo = transformGizmo;
			this.target = target;
		}

		public abstract void Execute();
		public abstract void UnExecute();
	}

	public class AddTargetCommand : SelectCommand
	{
		List<Transform> targetRoots = new List<Transform>();

		public AddTargetCommand(TransformGizmo transformGizmo, Transform target, List<Transform> targetRoots) : base(transformGizmo, target)
		{
			//Since we might have had a child selected and then selected the parent, the child would have been removed from the selected,
			//so we store all the targetRoots before we add so that if we undo we can properly have the children selected again.
			this.targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			transformGizmo.AddTarget(target, false);
		}

		public override void UnExecute()
		{
			transformGizmo.RemoveTarget(target, false);

			for(int i = 0; i < targetRoots.Count; i++)
			{
				transformGizmo.AddTarget(targetRoots[i], false);
			}
		}
	}

	public class RemoveTargetCommand : SelectCommand
	{
		public RemoveTargetCommand(TransformGizmo transformGizmo, Transform target) : base(transformGizmo, target) {}

		public override void Execute()
		{
			transformGizmo.RemoveTarget(target, false);
		}

		public override void UnExecute()
		{
			transformGizmo.AddTarget(target, false);
		}
	}

	public class ClearTargetsCommand : SelectCommand
	{
		List<Transform> targetRoots = new List<Transform>();

		public ClearTargetsCommand(TransformGizmo transformGizmo, List<Transform> targetRoots) : base(transformGizmo, null)
		{
			this.targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			transformGizmo.ClearTargets(false);
		}

		public override void UnExecute()
		{
			for(int i = 0; i < targetRoots.Count; i++)
			{
				transformGizmo.AddTarget(targetRoots[i], false);
			}
		}
	}

	public class ClearAndAddTargetCommand : SelectCommand
	{
		List<Transform> targetRoots = new List<Transform>();

		public ClearAndAddTargetCommand(TransformGizmo transformGizmo, Transform target, List<Transform> targetRoots) : base(transformGizmo, target)
		{
			this.targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			transformGizmo.ClearTargets(false);
			transformGizmo.AddTarget(target, false);
		}

		public override void UnExecute()
		{
			transformGizmo.RemoveTarget(target, false);

			for(int i = 0; i < targetRoots.Count; i++)
			{
				transformGizmo.AddTarget(targetRoots[i], false);
			}
		}
	}
	public struct ObjectData
	{
		public Transform parent;
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;
		public int catalogIndex;
			
		public ObjectData(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, int catalogIndex)
		{
			this.parent = parent;
			this.localPosition = localPosition;
			this.localRotation = localRotation;
			this.localScale = localScale;
			this.catalogIndex = catalogIndex;
		}

		public ObjectData(Transform transform)
		{
			this.parent = transform.parent;
			this.localPosition = transform.localPosition;
			this.localRotation = transform.localRotation;
			this.localScale = transform.localScale;
			this.catalogIndex = transform.GetComponent<BasicSerializableObject>()  ? transform.GetComponent<BasicSerializableObject>().CatalogIndex : -1;
		}
	}

	public class DeleteObjectCommand : SelectCommand
	{
		
		private ObjectData data;
		private Transform Target;
		
		public DeleteObjectCommand(TransformGizmo transformGizmo, Transform target) : base(transformGizmo, target)
		{
			Target = target;
			data = new ObjectData(target);
		}

		public override void Execute()
		{
			
			GameObject.Destroy(target.gameObject);
			transformGizmo.RemoveTarget(target, false);
		}

		public override void UnExecute()
		{
			GameObject go = null;
			if (data.catalogIndex == -1)
				go = GameObject.Instantiate(ObjectCatalog.Instance.Cassette, data.localPosition, data.localRotation, data.parent);
			else
				go = GameObject.Instantiate(SessionManager.Instance.Prefabs.Prefabs[data.catalogIndex], data.localPosition, data.localRotation, data.parent);

			go.transform.localPosition = data.localPosition;
			go.transform.localRotation = data.localRotation;
			go.transform.localScale = data.localScale;
			go.AddComponent<BasicSerializableObject>().CatalogIndex = data.catalogIndex;

			target = go.transform;
			data = new ObjectData(target);
			
			transformGizmo.AddTarget(go.transform, false);
		}
	}
	
	public class DuplicateCommand : SelectCommand
	{
		
		private Transform[] Target;
		private ObjectData[] data;
		
		public DuplicateCommand(TransformGizmo transformGizmo, Transform target, Transform[] targets) : base(transformGizmo, target)
		{
			Target = targets;
			data = new ObjectData[targets.Length];
			for (int i = 0; i< Target.Length; i++)
			{
				data[i] = new ObjectData(Target[i]);
			}
		}

		public override void Execute()
		{
			Transform[] newTargets = new Transform[data.Length];
			
			for (int i = 0; i<data.Length; i++)
			{
				GameObject go = GameObject.Instantiate(ObjectCatalog.Instance.Prefabs.Prefabs[data[i].catalogIndex], data[i].localPosition, data[i].localRotation, data[i].parent);
				go.transform.localPosition = data[i].localPosition;
				go.transform.localRotation = data[i].localRotation;
				go.transform.localScale = data[i].localScale;	
				
				go.AddComponent<BasicSerializableObject>().CatalogIndex = data[i].catalogIndex;
				newTargets[i] = go.transform;
			}
	
			Target = newTargets;
		}

		public override void UnExecute()
		{
			foreach (Transform target in Target)
			{
				GameObject.Destroy(target.gameObject);
			}
		}
	}
}
