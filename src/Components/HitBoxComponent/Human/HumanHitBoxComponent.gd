extends Area2D
class_name HumanHitBoxComponent

@export var shape : Shape2D
@export var health_component : HealthComponent
var inside_bodies : Array[Node2D] = []

func _ready():
	$CollisionShape.shape = self.shape

func _physics_process(delta):
	self.health_component.damage(process_damage())

func process_damage() -> float:
	# check for enemies inside of player
	var total_dmg = 0.0
	for enemy in self.inside_bodies:
		total_dmg += 5.0  # impl enemy dmg
	return total_dmg

func _on_hit_box_body_entered(body):
	self.inside_bodies.append(body)

func _on_hit_box_body_exited(body):
	self.inside_bodies.erase(body)
