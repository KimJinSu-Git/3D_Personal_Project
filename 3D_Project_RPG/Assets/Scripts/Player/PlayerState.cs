using Suntail;
using UnityEngine;

namespace suntail
{
    // 플레이어 상태 기준점.
    public abstract class PlayerBaseState
    {
        protected PlayerController player;

        public PlayerBaseState(PlayerController player)
        {
            this.player = player;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        // 이동
        protected void Move(float speedMultiplier = 1f)
        {
            Vector3 moveDirection = player.transform.forward * player.VerticalInput + player.transform.right * player.HorizontalInput;
            float currentSpeed = player.WalkSpeed * speedMultiplier;
            player.CharacterController.Move(moveDirection * (currentSpeed * Time.deltaTime));
        }

        // 중력
        protected void ApplyGravity()
        {
            if (player.IsGrounded && player.CharacterController.velocity.y < 0)
            {
                player.CharacterController.Move(Vector3.down * (2f * Time.deltaTime));
            }
            player.CharacterController.Move(Vector3.up * (player.Gravity * Time.deltaTime));
        }

        // 공격
        protected void Attack()
        {
            // 플레이어어가 무기를 들고 있고, 우클릭 버튼을 누르지 않는 상태에서, 좌클릭이 입력된다면 기본 공격 상태로 전환.
            if (Input.GetMouseButtonDown(0) && player.isHoldingWeapon && !Input.GetMouseButton(1))
            {
                player.ChangeState(PlayerState.Attack);
            } // 플레이어가 무기를 들고 있고, 우클릭 버튼을 누르고 있을 때, 좌클릭이 입력된다면 쉴드 공경 상태로 전환.
            else if (Input.GetMouseButtonDown(0) && player.isHoldingWeapon && Input.GetMouseButton(1))
            {
                player.ChangeState(PlayerState.ShieldAttack);
            }
        }
    }

    // 플레이어 기본 대기 상태
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.playerAnimator.Play(player.isHoldingWeapon ? "Idle1" : "nIdle1");
        }

        public override void Update()
        {
            ApplyGravity();
            WeaponSwap();
            Attack();

            if (Input.GetMouseButtonDown(1))
            {
                player.ChangeState(PlayerState.ShieldWait);
            }

            if (Mathf.Abs(player.VerticalInput) > 0.1f || Mathf.Abs(player.HorizontalInput) > 0.1f)
            {
                player.ChangeState(PlayerState.Walk);
            }
            else if (Input.GetKey(player.JumpKey) && player.IsGrounded)
            {
                player.ChangeState(PlayerState.Jump);
            }
        }

        public override void Exit()
        {
            
        }
        
        private void WeaponSwap()
        {
            if (Input.GetKeyDown(KeyCode.F))
            { 
                // true 라면 무기를 들고 있는것이고, false라면 무기를 등에 돌려 넣은것
                switch (player.isHoldingWeapon)
                {
                    case true:
                        player.playerAnimator.Play("Sheathe");
                        player.isHoldingWeapon = false;
                        break;
                    case false:
                        player.playerAnimator.Play("UnSheathe");
                        player.isHoldingWeapon = true;
                        break;
                }
            }
        }
    }

    // 플레이어 걷는 상태
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerController player) : base(player) { }

        public override void Enter()
        {
           
        }

        public override void Update()
        {
            ApplyGravity();
            Move();
            Attack();
            
            if (Input.GetMouseButtonDown(1))
            {
                player.ChangeState(PlayerState.ShieldWalk);
            }
            
            switch (player.VerticalInput)
            {
                case > 0.1f:
                    player.playerAnimator.Play(player.isHoldingWeapon ? "Walk_F" : "nWalk_F");
                    break;
                case < -0.1f:
                    player.playerAnimator.Play(player.isHoldingWeapon ? "Walk_B" : "nWalk_B");
                    break;
                case 0:
                    player.ChangeState(PlayerState.Idle);
                    break;
            }

            if (player.IsRunning && Mathf.Abs(player.VerticalInput) > 0.1f)
            {
                player.ChangeState(PlayerState.Run);
            }
            
            if (Input.GetKey(player.JumpKey) && player.IsGrounded)
            {
                player.ChangeState(PlayerState.Jump);
            }
        }

        public override void Exit()
        {
            
        }
    }

    // 플레이어 달리는 상태
    public class PlayerRunState : PlayerBaseState
    {
        public PlayerRunState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            
        }

        public override void Update()
        {
            ApplyGravity();
            Move(player.RunMultiplier);
            Attack();
            
            if (Input.GetMouseButtonDown(1))
            {
                player.ChangeState(PlayerState.ShieldRun);
            }
            
            switch (player.VerticalInput)
            {
                case > 0.1f:
                    player.playerAnimator.Play(player.isHoldingWeapon ? "Run_F" : "nRun_F");
                    break;
                case < -0.1f:
                    player.playerAnimator.Play(player.isHoldingWeapon ? "Run_B" : "nRun_B");
                    break;
                case 0:
                    player.ChangeState(PlayerState.Idle);
                    break;
            }

            if (player.IsRunning == false)
            {
                player.ChangeState(PlayerState.Walk);
            }
            
            if (Input.GetKey(player.JumpKey) && player.IsGrounded)
            {
                player.ChangeState(PlayerState.Jump);
            }
        }

        public override void Exit() { }
    }

    // 플레이어 점프 상태
    public class PlayerJumpState : PlayerBaseState
    {
        private float verticalVelocity;

        public PlayerJumpState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            if (player.isHoldingWeapon)
            {
                player.playerAnimator.Play("Jump_In");
            }
            else
            {
                player.playerAnimator.Play("nJump_In");
            }
            
            verticalVelocity = Mathf.Sqrt(player.JumpForce * -2f * player.Gravity);
        }

        public override void Update()
        {
            verticalVelocity += player.Gravity * Time.deltaTime;
            Vector3 moveDirection = (player.transform.forward * player.VerticalInput + player.transform.right * player.HorizontalInput) * (player.WalkSpeed * Time.deltaTime);
            moveDirection.y = verticalVelocity * Time.deltaTime;
            player.CharacterController.Move(moveDirection);

            if (player.IsGrounded && verticalVelocity < 0)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }

        public override void Exit() { }
    }

    // 기본 공격 콤보
    public class PlayerAttack : PlayerBaseState
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private bool comboAttackSuccess;
        public PlayerAttack(PlayerController player) : base(player) {}

        public override void Enter()
        {
            comboAttackSuccess = false;
            player.playerAnimator.Play("Attack1");
        }

        public override void Update()
        {
            AnimatorStateInfo playerStateInfo = player.playerAnimator.GetCurrentAnimatorStateInfo(0);

            if (playerStateInfo.normalizedTime < 0.25f)
            {
                comboAttackSuccess = false;
            }
            
            // 애니메이션 시간이 0.3~0.7 사이에 좌클릭이 입력된다면.
            if (playerStateInfo.normalizedTime is > 0.3f and < 0.7f && Input.GetMouseButtonDown(0))
            {
                // 다음 콤보 공격으로 넘어가라.
                player.playerAnimator.SetTrigger(Attack);
                comboAttackSuccess = true;
            }

            // 콤보공격의 마지막 공격인 5번째 공격이고, 애니메이션 시간이 0.9가 지났다면 Idle로 전환 !
            if (playerStateInfo.IsName("Attack5") && playerStateInfo.normalizedTime > 0.9)
            {
                player.ChangeState(PlayerState.Idle);
            }
            
            // 애니메이션의 재생시간이 0.9초가 넘었음에도 콤보 공격이 입력되지 않았다면
            if (playerStateInfo.normalizedTime > 0.9 && !comboAttackSuccess)
            {
                // Idle 상태로 전환하라.
                player.ChangeState(PlayerState.Idle);
            }
        }

        public override void Exit()
        {
            
        }
    }
    
    // 방패 들고 가만히 있는 상태
    public class ShieldWait : PlayerBaseState
    {
        public ShieldWait(PlayerController player) : base(player) {}

        public override void Enter()
        {
            player.playerAnimator.Play("ShieldBlock_Wait");
        }

        public override void Update()
        {
            Move();
            Attack();
            
            if (!(Input.GetMouseButton(1)))
            {
                player.ChangeState(PlayerState.Idle);
            }

            switch (player.VerticalInput)
            {
                case > 0.1f:
                    player.ChangeState(PlayerState.ShieldWalk);
                    break;
                case < 0f:
                    player.VerticalInput = 0;
                    break;
            }
        }

        public override void Exit()
        {
            
        }
    }
    
    // 방패 들고 앞으로 걷는 상태
    public class ShieldWalk : PlayerBaseState
    {
        public ShieldWalk(PlayerController player) : base(player) {}

        public override void Enter()
        {
            
        }

        public override void Update()
        {
            Move();
            Attack();
            
            if (!(Input.GetMouseButton(1)))
            {
                player.ChangeState(PlayerState.Walk);
            }
            
            switch (player.VerticalInput)
            {
                case > 0.1f:
                    if (player.IsRunning)
                    {
                        player.ChangeState(PlayerState.ShieldRun);
                    }
                    else
                    {
                        player.playerAnimator.Play("ShieldBlock_Walk");
                    }
                    break;
                case < 0f:
                    player.VerticalInput = 0;
                    break;
            }
        }

        public override void Exit()
        {
            
        }
    }
    
    // 방패 들고 앞으로 뛰는 상태
    public class ShieldRun : PlayerBaseState
    {
        public ShieldRun(PlayerController player) : base(player) {}

        public override void Enter()
        {
            
        }

        public override void Update()
        {
            Move(player.RunMultiplier);
            Attack();
            
            if (!(Input.GetMouseButton(1)))
            {
                player.ChangeState(PlayerState.Run);
            }
            
            switch (player.VerticalInput)
            {
                case > 0.1f:
                    if (player.IsRunning)
                    {
                        player.playerAnimator.Play("ShieldBlock_Run");
                    }
                    else
                    {
                        player.ChangeState(PlayerState.ShieldWalk);
                    }
                    break;
                case < -0.1f:
                    player.VerticalInput = 0;
                    break;
            }
        }

        public override void Exit()
        {
            
        }
    }
    
    // 방패로 공격하는 콥보 
    public class ShieldAttack : PlayerBaseState
    {
        private bool comboAttackSuccess;
        public ShieldAttack(PlayerController player) : base(player) {}

        public override void Enter()
        {
            comboAttackSuccess = false;
            player.playerAnimator.Play("ShieldAttack1");
        }

        public override void Update()
        {
            AnimatorStateInfo playerStateInfo = player.playerAnimator.GetCurrentAnimatorStateInfo(0);

            if (playerStateInfo.normalizedTime < 0.25f)
            {
                comboAttackSuccess = false;
            }
            
            // 애니메이션 시간이 0.3~0.7 사이에 우클릭을 누른 채로 좌클릭이 입력된다면.
            if (playerStateInfo.normalizedTime is > 0.3f and < 0.7f && Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
            {
                // 다음 콤보 공격으로 넘어가라.
                player.playerAnimator.SetTrigger("ShieldAttack");
                comboAttackSuccess = true;
            }

            // 콤보공격의 마지막 공격인 3번째 공격이고, 애니메이션 시간이 0.9가 지났을 때
            if (playerStateInfo.IsName("ShieldAttack3") && playerStateInfo.normalizedTime > 0.9)
            {
                // 우클릭을 누르고 있다면
                if (Input.GetMouseButton(1))
                {
                    // 쉴드 대기 상태로 전환
                    player.ChangeState(PlayerState.ShieldWait);
                }
                else
                {
                    // 안 누르고 있다면 Idle 상태로 전환
                    player.ChangeState(PlayerState.Idle);
                }
            }
            
            // 애니메이션의 재생시간이 0.9초가 넘었음에도 콤보 공격이 입력되지 않았다면
            if (playerStateInfo.normalizedTime > 0.9 && !comboAttackSuccess)
            {
                // Idle 상태로 전환하라.
                player.ChangeState(PlayerState.Idle);
            }
        }

        public override void Exit()
        {
            
        }
    }
}