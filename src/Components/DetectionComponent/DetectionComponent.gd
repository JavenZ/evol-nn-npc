extends Area2D
class_name DetectionComponent

@export var radius : float = 750.0
@export var body_detected : Node2D

func _ready():
	# update detection shape radius
	if self.radius > 0.0:
		$CollisionShape.shape.radius = self.radius

func _on_detection_area_body_entered(_body):
	# uses collision layer to detect
	print("player detected!")
	self.body_detected = _body

func _on_detection_area_body_exited(_body):
	self.body_detected = null
