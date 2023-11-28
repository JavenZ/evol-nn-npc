extends Node
class_name HealthComponent

@export var healthbar_component : HealthBarComponent
@export var character_component : CharacterComponent
@export var health: float = 100.0
@export var max_health: float = 100.0
@export var damage_cooldown : float = 0.0

var invincible : bool = false

signal hit

func _ready():
	# init health bar
	self.update_health_bar()

func update_health_bar() -> void:
	self.healthbar_component.update(self.max_health, self.health)

func damage(amount: float) -> void:
	"""
	Applies damage to character then updates healthbar and emits hit signal.
	"""
	# is the character already dead?
	if self.character_component.state == self.character_component.States.DEAD:
		return
	
	# apply damage?
	if not self.invincible and amount > 0.0:
		# print(self.name, " taking damage: ", amount)
		# subtract health
		self.health = maxf(0.0, self.health - amount)
		
		# begin damage cooldown timer
		$DamageCooldownTimer.start(self.damage_cooldown)
		self.invincible = true
	
		# health bar display
		update_health_bar()
		
		# emit hit signal
		self.hit.emit()

func _on_damage_cooldown_timer_timeout():
	# print(self.name, " damage cooldown expired.")
	self.invincible = false
