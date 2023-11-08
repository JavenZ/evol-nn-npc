extends Area2D

@export var radius : float = 750.0
var body_detected : Node2D

func _ready():
	# update detection shape radius
	if self.radius > 0.0:
		$CollisionShape.shape.radius = self.radius

func _on_detection_area_body_entered(_body):
	# uses collision layer to detect
	self.body_detected = _body

func _on_detection_area_body_exited(_body):
	self.body_detected = null
