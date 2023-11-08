extends Area2D


@export var shape : Shape2D

func _ready():
	$CollisionShape.shape = self.shape
