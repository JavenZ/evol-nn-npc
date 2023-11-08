extends CharacterBody2D
class_name CharacterComponent

# NODE REFERENCES
@export var healthbar_component : HealthBarComponent
@export var collision_component : CollisionComponent
@export var sprite_component : SpriteComponent
@export var damage_component : DamageProcess
@export var movement_component : MovementProcess

# CHARACTER STATE VARS -------------------------- #
@export var health: float = 100.0
@export var max_health: float = 100.0
@export var mana: float = 100.0
@export var max_mana: float = 100.0
@export var dead : bool = false
@export var damage_cooldown : float = 0.0
@export var invincible : bool = false
# ------------------------------------------ #

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
var is_jumping := false
var gravity_acceleration : float = 3840
var gravity_max : float = 1020
var jump_coyote_timer : float = 0
var jump_buffer_timer : float = 0


func _ready():
	# add character to groups
	add_to_group("Characters")
	
	# init health bar
	self.update_health_bar()
	
	# reset invincibility state
	# $DamageCooldownTimer.start(self.damage_cooldown) ?
	self.invincible = false

func _physics_process(delta):
	# callback processes
	var dmg = self.damage_component.process_damage()
	self.take_damage(dmg)
	var mv = self.movement_component.process_movement()
	self.move(mv, delta)

func move(input: Dictionary, delta: float) -> void:
	# calculate horizontal velocity
	x_movement(input, delta)
	
	# calculate vertical velocity
	jump_logic(input, delta)
	apply_gravity(delta)
	
	# update movement timers
	update_movement_timers(delta)
#
	# apply movement and slide from collisions
	move_and_slide()

	# determine the new animation state and update callbacks
	select_animation_state()

func x_movement(input: Dictionary, delta: float) -> void:
	var x_dir = input["x"]

	# Stop if we're not doing movement inputs.
	if x_dir == 0: 
		velocity.x = Vector2(velocity.x, 0).move_toward(Vector2(0,0), deceleration * delta).x
		return
	
	# If we are doing movement inputs and above max speed, don't accelerate nor decelerate
	# Except if we are turning (This keeps our momentum gained from outside or slopes)
	if abs(velocity.x) >= max_speed and sign(velocity.x) == x_dir:
		return
	
	# Are we turning? Deciding between acceleration and turn_acceleration
	var accel_rate : float = acceleration if sign(velocity.x) == x_dir else turning_acceleration
	
	# Accelerate
	velocity.x += x_dir * accel_rate * delta
	
	# Change sprite direction
	self.sprite_component.set_direction(x_dir)

func select_animation_state() -> void:
	if is_on_floor() and self.velocity.x != 0:
		# walk animation
		self.sprite_component.update_animation_state("walk")
	elif is_jumping:
		# jump animation
		self.sprite_component.update_animation_state("jump")
	else:
		self.sprite_component.update_animation_state("idle")

func jump_logic(input: Dictionary, _delta: float) -> void:
	# Reset our jump requirements
	if is_on_floor():
		jump_coyote_timer = jump_coyote
		is_jumping = false
	if input["just_jump"]:
		jump_buffer_timer = jump_buffer
	
	# Jump if grounded, there is jump input, and we aren't jumping already
	if jump_coyote_timer > 0 and jump_buffer_timer > 0 and not is_jumping:
		is_jumping = true
		jump_coyote_timer = 0
		jump_buffer_timer = 0
		# If falling, account for that lost speed
		if velocity.y > 0:
			velocity.y -= velocity.y
		velocity.y = -jump_force
	
	# Cut the velocity if let go of jump. This means our jumpheight is varaiable
	if input["released_jump"] and velocity.y < 0:
		velocity.y -= (jump_cut * velocity.y)
	
	# This way we won't start slowly descending / floating once hit a ceiling
	if is_on_ceiling(): velocity.y = jump_hang_treshold + 100.0

func apply_gravity(delta: float) -> void:
	var applied_gravity : float = 0
	
	# No gravity if we are grounded
	if jump_coyote_timer > 0:
		return
	
	# Normal gravity limit
	if velocity.y <= gravity_max:
		applied_gravity = gravity_acceleration * delta
	
	# If moving upwards while jumping, the limit is jump_gravity_max to achieve lower gravity
	if (is_jumping and velocity.y < 0) and velocity.y > jump_gravity_max:
		applied_gravity = 0
	
	# Lower the gravity at the peak of our jump (where velocity is the smallest)
	if is_jumping and abs(velocity.y) < jump_hang_treshold:
		applied_gravity *= jump_hang_gravity_mult
	
	velocity.y += applied_gravity

func update_movement_timers(delta: float) -> void:
	jump_coyote_timer -= delta
	jump_buffer_timer -= delta

func update_health_bar() -> void:
	self.healthbar_component.update(self.max_health, self.health)

func take_damage(amount: float) -> void:
	"""
	Applies damage to character then updates healthbar and death state.
	"""
	# is player already dead?
	if self.dead:
		return
	
	# apply damage?
	if not self.invincible and amount > 0.0:
		print(self.name, " taking damage: ", amount)
		# subtract health
		self.health = maxf(0.0, self.health - amount)
		
		# begin cooldown timer
		$DamageCooldownTimer.start(self.damage_cooldown)
		self.invincible = true
	
		# health bar display
		self.update_health_bar()
	
	# death? ignores invincibility if character has no health
	if self.health <= 0.0:
		print(self.name, " died.")
		self.dead = true

func _on_damage_cooldown_timer_timeout():
	print(self.name, " damage cooldown expired.")
	self.invincible = false
