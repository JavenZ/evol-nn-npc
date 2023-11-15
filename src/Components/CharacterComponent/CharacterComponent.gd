extends CharacterBody2D
class_name CharacterComponent

# NODE REFERENCES
@export var collision_component : CollisionComponent
@export var sprite_component : SpriteComponent
@export var brain_component: MobBrainComponent
@export var health_component : HealthComponent

# MOVEMENT VARS ---------------- #
@export var max_speed: float = 560
@export var acceleration: float = 2880
@export var turning_acceleration : float = 9600
@export var deceleration: float = 3200
@export var jump_force : float = 1100
var jump_cut : float = 0.25
var jump_gravity_max : float = 500
var jump_hang_treshold : float = 2.0
var jump_hang_gravity_mult : float = 0.1
var jump_coyote : float = 0.08
var jump_buffer : float = 0.1
var gravity_acceleration : float = 3840
var gravity_max : float = 1020
var jump_coyote_timer : float = 0
var jump_buffer_timer : float = 0

# An enum allows us to keep track of valid states.
enum States {IDLE, WALK, JUMP, ATTACK, DEAD}
@export var state : States = States.IDLE

func update_state(new_state: States) -> void:
	self.state = new_state
	if new_state == States.IDLE:
		self.sprite_component.update_animation_state("idle")
	elif new_state == States.WALK:
		self.sprite_component.update_animation_state("walk")
	elif new_state == States.ATTACK:
		self.sprite_component.update_animation_state("attack")
	elif new_state == States.DEAD:
		self.sprite_component.update_animation_state("dead")

func _ready():
	# update name
	set_name.call_deferred(get_parent().name)
	
	# add character to groups
	add_to_group("Characters")
	
	# connect to death signal
	self.health_component.death.connect(death)

func _physics_process(delta):
	var input = self.brain_component.next_move()
	move(input, delta)

func move(input: Dictionary, delta: float) -> void:
	# block input if character is dead
	if self.state != States.DEAD:
		
		if self.state == States.IDLE:
			# attack
			attack(input)
			
		if self.state != States.ATTACK:
			# vertical movement
			jump(input, delta)
			
			# horizontal movement
			walk(input, delta)
			
			# idle movement
			idle()
	
	# apply gravity
	gravity(delta)
	
	# update jump timers
	update_movement_timers(delta)
#
	# apply movement and slide from collisions
	move_and_slide()

func death():
	self.update_state(States.DEAD)

func attack(input: Dictionary):
	# if has attack component
	if input['attack'] == true:
		self.update_state(States.ATTACK)

func idle():
	# update state to idle if not moving or falling
	if is_on_floor() and self.velocity.x == 0:
		self.update_state(States.IDLE)

func walk(input: Dictionary, delta: float):
	# x input
	var x_dir = input["x"]

	# decelerate if we're not doing movement inputs
	if x_dir == 0: 
		velocity.x = Vector2(velocity.x, 0).move_toward(Vector2(0,0), deceleration * delta).x
		return
	
	# if above max speed, don't accelerate or decelerate
	if abs(velocity.x) >= max_speed and sign(velocity.x) == x_dir:
		return
	
	# Are we turning? Deciding between acceleration and turn_acceleration
	var accel_rate : float = acceleration if sign(velocity.x) == x_dir else turning_acceleration
	
	# Accelerate
	velocity.x += x_dir * accel_rate * delta
	
	# Change sprite direction
	self.sprite_component.set_direction(x_dir)
	
	# update state to walk if moving but not falling
	if is_on_floor() and self.velocity.x != 0:
		self.update_state(States.WALK)

func jump(input: Dictionary, _delta: float) -> void:
	# Reset our jump requirements
	if is_on_floor():
		jump_coyote_timer = jump_coyote
		# is_jumping = false
	if input["jump"]:
		jump_buffer_timer = jump_buffer
	
	# Jump if grounded, there is jump input, and we aren't jumping already
	if jump_coyote_timer > 0 and jump_buffer_timer > 0 and self.state != States.JUMP:
		jump_coyote_timer = 0
		jump_buffer_timer = 0
		# If falling, account for that lost speed
		if velocity.y > 0:
			velocity.y -= velocity.y
		velocity.y = -jump_force
		
		# update state to jump
		self.update_state(States.JUMP)
	
	# This way we won't start slowly descending / floating once hit a ceiling
	if is_on_ceiling():
		velocity.y = jump_hang_treshold + 100.0

func gravity(delta: float) -> void:
	var applied_gravity : float = 0
	
	# No gravity if we are grounded
	if jump_coyote_timer > 0:
		return
	
	# Normal gravity limit
	if velocity.y <= gravity_max:
		applied_gravity = gravity_acceleration * delta
	
	# If moving upwards while jumping, the limit is jump_gravity_max to achieve lower gravity
	if (self.state == States.JUMP and velocity.y < 0) and velocity.y > jump_gravity_max:
		applied_gravity = 0
	
	# Lower the gravity at the peak of our jump (where velocity is the smallest)
	if self.state == States.JUMP and abs(velocity.y) < jump_hang_treshold:
		applied_gravity *= jump_hang_gravity_mult
	
	velocity.y += applied_gravity

func update_movement_timers(delta: float) -> void:
	jump_coyote_timer -= delta
	jump_buffer_timer -= delta
