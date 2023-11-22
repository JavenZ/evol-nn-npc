extends TileMap
class_name TileMapComponent

const LAYER_WALL = 0
const LAYER_NAV = 1

func get_random_nav_tile():
	return self.get_random_tile(LAYER_NAV)

func get_random_tile(layer_id: int):
	var tile = Util.choose(self.get_used_cells(layer_id))
	return self.map_to_local(tile)

