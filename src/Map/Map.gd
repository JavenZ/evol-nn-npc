extends Node2D
class_name Map

@export var tile_map : TileMapComponent

func spawn_character(char: CharacterComponent):
	self.add_child(char)
	char.position = self.tile_map.get_random_nav_tile()
	char.brain_component.tilemap_component = self.tile_map
