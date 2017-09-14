using System.Collections.Generic;
using UnityEngine;

namespace Damage.Effects {
    public abstract class Effect : MonoBehaviour {
        public List<int> AffectedTeams;
    }
}