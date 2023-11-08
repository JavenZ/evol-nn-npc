extends ProgressBar

@export var force_visibility : bool = false

@onready var collision_capsule : CollisionShape2D = get_node("../CollisionShape")

func _ready():
	"""
	Calculates and updates the initial healthbar transform values.
	"""
	# extract capsule shape object from collision capsule
	var capsule_shape = self.collision_capsule.shape

	# calculate bar size
	self.size.y = 15
	self.size.x = capsule_shape.radius * 2
	
	# calculate bar position (offset for centered positioning)
	self.position.x = self.collision_capsule.position.x - (self.size.x / 2)
	self.position.y = self.collision_capsule.position.y + (capsule_shape.height / 2) + 3

func update(max_v: float, cur_v: float) -> void:
	"""
	Updates healthbar progress percentage.
	"""
	self.max_value = max_v
	self.value = cur_v
	
	# hide health bar?
	if !self.force_visibility:
		if self.value >= self.max_value:
			self.visible = false
		else:
			self.visible = true
