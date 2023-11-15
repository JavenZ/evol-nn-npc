extends Area2D
class_name HitBoxComponent

@export var shape : Shape2D
@export var health_component : HealthComponent
var inside_attacks : Array[AttackComponent] = []

func _ready():
	$CollisionShape.shape = self.shape

func _physics_process(delta):
	if !inside_attacks.is_empty():
		self.health_component.damage(process_damage())

func process_damage() -> float:
	# calculate total attack damage
	var total_dmg = 0.0
	for attack in self.inside_attacks:
		total_dmg += calculate_damage(attack)
	return total_dmg

func calculate_damage(attack: AttackComponent) -> float:
	return attack.damage

func _on_hit_box_body_entered(body):
	print(name, ": body entered hitbox (", body.name, ")")
	if body is AttackComponent:
		self.inside_attacks.append(body as AttackComponent)

func _on_hit_box_body_exited(body):
	self.inside_attacks.erase(body)
