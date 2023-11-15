extends StaticBody2D
class_name AttackComponent

@export var shape : CollisionShape2D
@export var damage : float = 10.0
@export var knockback : float = 0.0
@export var start_disabled : bool = true

func _ready():
	if self.start_disabled:
		stop_attack()

func start_attack():
	shape.disabled = false

func stop_attack():
	shape.disabled = true
