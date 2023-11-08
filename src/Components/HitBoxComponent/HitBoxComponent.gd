extends Area2D
class_name HitBoxComponent

@export var shape : Shape2D
var inside_bodies : Array[Node2D] = []

func _ready():
	$CollisionShape.shape = self.shape

func _on_hit_box_body_entered(body):
	self.inside_bodies.append(body)

func _on_hit_box_body_exited(body):
	self.inside_bodies.erase(body)
