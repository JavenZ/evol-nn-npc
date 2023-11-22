extends Node2D
class_name Map

@export var tile_map : TileMapComponent

func get_random_nav_tile():
	return self.tile_map.get_random_nav_tile()

func spawn_character(char: CharacterComponent):
	char.position = self.tile_map.get_random_nav_tile()
	char.brain_component.map = self
	self.add_child(char)
