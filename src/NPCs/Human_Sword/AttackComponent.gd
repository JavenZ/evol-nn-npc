extends StaticBody2D
class_name AttackComponent

@export var damage : float = 10.0
@export var knockback : float = 0.0

func _ready():
	stop_attack()

func start_attack():
	$CollisionShape.disabled = false

func stop_attack():
	$CollisionShape.disabled = true
