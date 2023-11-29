extends StaticBody2D
class_name AttackComponent

@export var shape : CollisionShape2D
@export var damage : float = 10.0
@export var knockback : float = 0.0
@export var start_disabled : bool = true
@export var attack_cooldown : float = 1.0

var cooling_down : bool = false

func _ready():
	if self.start_disabled:
		stop_attack()

func start_attack() -> bool:
	if not self.cooling_down:
		shape.disabled = false
		
		# begin attack cooldown timer
		$AttackCooldownTimer.start(self.attack_cooldown)
		self.cooling_down = true
		return true
	return false

func stop_attack():
	shape.disabled = true

func _on_attack_cooldown_timer_timeout():
	self.cooling_down = false
