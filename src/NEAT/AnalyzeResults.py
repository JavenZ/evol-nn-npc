import pandas as pd
import matplotlib.pyplot as plt


def analyze():
	# prepare dataframe
	file_path = "./Saves/training_results.csv"
	df = pd.read_csv(file_path)
	df.set_index(['Batch', 'Gen'], inplace=True)
	gens = ([i for i in range(len(df.index.unique()))] * 2)
	gens.sort()
	df['Generation'] = gens
	df.set_index('Generation', drop=True, inplace=True)

	# plot dataframe
	ax = df.groupby('NPC')['Fit_Mean'].plot(
		title="NEAT Training Results (Mean Fitness)",
		grid=True, legend=True, ylabel="Mean Fitness"
	)
	plt.show()
	ax[0].get_figure().savefig("Fit_Mean.png")

	ax = df.groupby('NPC')['Fit_Best'].plot(
		title="NEAT Training Results (Best Fitness)",
		grid=True, legend=True,ylabel="Best Fitness"
	)
	plt.show()
	ax[0].get_figure().savefig("Fit_Best.png")

	ax = df.groupby('NPC')['Complexity_Mean'].plot(
		title="NEAT Training Results (Mean Complexity)",
		grid=True, legend=True, ylabel="Mean Complexity"
	)
	plt.show()
	ax[0].get_figure().savefig("Complexity_Mean.png")


if __name__ == "__main__":
	analyze()
