"""
Axis:
first - direction to down
second - direction to right
"""
from copy import deepcopy
import random


def up():
    return -1, 0

def right():
    return 0, 1

def down():
    return 1, 0

def left():
    return 0, -1


class Scene(list):

    def __init__(self, *args, **kwargs):
        super(Scene, self).__init__(*args, **kwargs)
        self.first_size = len(self)
        self.second_size = len(self[0])

    def get_cell(self, first, second):
        if 0 <= first < self.first_size and 0 <= second < self.second_size:
            return self[first][second]

    def step(self, player, steps, deep=0):
        if deep > 10:
            return []
        results = []
        for direction in [up, right, down, left]:
            vDirection = direction()
            new_scene = deepcopy(self)

            cell = new_scene.get_cell(player[0] + vDirection[0], player[1] + vDirection[1])
            if cell in ['#', 'x']:
                if steps:
                    last_step = steps[-1]
                    prev_direction = globals()[last_step]()
                    if prev_direction[0] == -vDirection[0] and prev_direction[1] == -vDirection[1]:
                        continue
            else:
                new_scene[player[0]][player[1]] = ' '

                np_first = player[0]
                np_second = player[1]
                while 1:
                    np_first -= vDirection[0]
                    np_second -= vDirection[1]
                    cell = new_scene.get_cell(np_first, np_second)
                    if not cell:
                        break
                    if cell == ' ':
                        continue
                    np_first += vDirection[0]
                    np_second += vDirection[1]
                    new_scene[np_first][np_second] = '#'
                    break

            np_first = player[0]
            np_second = player[1]
            while 1:
                np_first += vDirection[0]
                np_second += vDirection[1]
                cell = new_scene.get_cell(np_first, np_second)
                if not cell:
                    break
                if cell == ' ':
                    continue
                new_steps = steps + [direction.__name__]
                if cell == '#':
                    results.extend(new_scene.step((np_first, np_second), new_steps, deep+1))
                else:
                    results.append(new_steps)
                    #print '\n'.join(''.join(i) for i in new_scene)
                    #print
                break
        return results

    @staticmethod
    def load(raw):
        rows = raw.split('\n')
        cols_count = max(map(len, rows))
        rows = [list(row + ' ' * (cols_count-len(row)))
                for row in rows]
        return Scene(rows)


def get_rnd_scene(first_size, second_size, iters):
    scene = [[' '] * second_size for i in range(first_size)]
    for i in xrange(iters):
        scene[random.randint(0, first_size-1)][random.randint(0, second_size-1)] = '#'
    scene[random.randint(0, first_size-1)][random.randint(0, second_size-1)] = 'x'
    return Scene(scene)

def main():
    while 1:
        scene = get_rnd_scene(8, 8, 8)
        start_points = []
        for i, row in enumerate(scene):
            for j, cell in enumerate(row):
                if cell == '#':
                    start_points.append((i, j))

        totals = []
        for start_point in start_points:
            result = scene.step(start_point, [])
            if result:
                cnts = [len(i) for i in result]
                if min(cnts) > 4 and len(result) > 5:
                    totals.append((start_point, result))

        if totals:
            for start_point, result in totals:
                n_scene = deepcopy(scene)
                n_scene[start_point[0]][start_point[1]] = 'o'
                print '\n'.join(''.join(i) for i in n_scene)
                print
                for i in result:
                    print ', '.join(i)
                print
            break


if __name__ == '__main__':
    main()
