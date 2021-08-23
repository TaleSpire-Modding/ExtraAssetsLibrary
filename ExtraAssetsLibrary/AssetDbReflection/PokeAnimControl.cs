using UnityEngine;

namespace PokeSpire.PokeSpire
{
    public class PokeAnimControl : MonoBehaviour
    {
        private Animation anim;

        private bool dead = false;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animation>();
        }

        private void LoadAnim()
        {
            if (anim == null) anim = GetComponent<Animation>();
        }

        // Update is called once per frame
        void Update()
        {
            if (anim.isPlaying) return;
            if (!dead) Idle();
        }

        public void Faint()
        {
            LoadAnim();
            anim["17"].speed = 1;
            anim.Play("17");
            dead = true;
        }

        public void Idle()
        {
            LoadAnim();
            anim.Play("0");
        }

        public void Revive()
        {
            LoadAnim();
            anim["17"].speed = -1;
            anim["17"].time = anim["17"].length;
            anim.Play("17");
            dead = false;
        }

        public void Attack1()
        {
            LoadAnim();
            anim.Play("1");
        }

        public void Attack2()
        {
            LoadAnim();
            anim.Play("3");
        }
        public void Attack3()
        {
            LoadAnim();
            anim.Play("8");
        }
        public void Attack4()
        {
            LoadAnim();
            anim.Play("9");
        }

        public void Attack5()
        {
            LoadAnim();
            anim.Play("12");
        }

        public void TakeDamage1()
        {
            LoadAnim();
            anim.Play("13");
        }

        public void TakeDamage2()
        {
            LoadAnim();
            anim.Play("14");
        }

    }

}
