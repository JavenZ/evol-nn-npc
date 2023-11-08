extends Area2D

@export var radius : float = 750.0

func _ready():
	# update detection shape radius
	if self.radius > 0.0:
		$CollisionShape.shape.radius = self.radius
