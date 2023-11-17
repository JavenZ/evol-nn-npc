extends Node2D
class_name Game

signal finished
signal test2

@export var map : Node2D
@export var team_a : Array[CharacterComponent]
@export var team_b : Array[CharacterComponent]

var _finished : bool = false

func _ready():
	for node in self.team_a:
		node.add_to_group("TeamA")
		node.connect("death", on_character_death)
	for node in self.team_b:
		node.add_to_group("TeamB")
		node.connect("death", on_character_death)

func _physics_process(delta):
	if !self._finished:
		var a_size = len(get_tree().get_nodes_in_group("TeamA"))
		var b_size = len(get_tree().get_nodes_in_group("TeamB"))
		if a_size == 0 or b_size == 0:
			print("A team has been defeated!")
			finished.emit()
			self._finished = true
			# get_tree().quit()

func on_character_death(character):
	self.map.remove_child(character)
