using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Taki.Utility.Core;
using System;

namespace Taki.Utility
{
    public enum UserInputLockTag
    {
        Normal,   
        PauseMenu, 
    }

    public sealed class UserInputLockEntrypoint : MonoBehaviour
    {
        [Serializable]
        public struct LockTarget
        {
            public UserInputLockTag Tag;
            public Transform Parent;
        }

        [SerializeField] private List<LockTarget> _lockTargets = new();

        private Dictionary<UserInputLockTag, List<IUserInputLock>> _userInputLocksByTag;

        public void CollectUserInputLocks()
        {
            _userInputLocksByTag = _lockTargets
                .Where(target => target.Parent != null)
                .GroupBy(target => target.Tag)
                .ToDictionary(
                    group => group.Key,
                    group => group.SelectMany(target => target.Parent.GetComponentsInChildren<IUserInputLock>(true))
                                  .ToList()
                );
        }

        public void SetAllUserInputLocks(bool isLocked)
        {
            Thrower.IfNull(_userInputLocksByTag, nameof(_userInputLocksByTag));

            foreach (var locks in _userInputLocksByTag.Values)
            {
                locks.ForEach(handler => handler.SetInputLock(isLocked));
            }
        }

        public void SetUserInputLockByTag(UserInputLockTag tag, bool isLocked)
        {
            Thrower.IfNull(_userInputLocksByTag, nameof(_userInputLocksByTag));

            if (_userInputLocksByTag.TryGetValue(tag, out var handlers))
            {
                handlers.ForEach(handler => handler.SetInputLock(isLocked));
            }
            else
            {
                Debug.LogWarning($"UserInputLockTag '{tag}' ‚É‘Î‰‚·‚é“ü—ÍƒƒbƒN‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½B");
            }
        }
    }
}