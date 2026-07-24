import test from 'node:test';
import assert from 'node:assert/strict';
import { mapLogEntries } from './map-log-data.mjs';

test('maps source entries into the baby-log shape for feeding data', () => {
  const template = {
    id: '1',
    trackerType: 'feeding',
    createdAt: '2026-07-23T07:46:00',
    updatedAt: '2026-07-23T07:46:00',
    data: {
      bottleSize: 4,
      feedingType: 'bottle',
      breastSide: [],
      milkType: ['breastmilk'],
      milkConsumed: 3,
      startTime: '2026-07-23T07:30:00',
      endTime: '2026-07-23T07:45:00',
      notes: 'Finished most of the bottle.'
    }
  };

  const sourceEntries = [
    {
      bottleSize: 4,
      feedingType: 'bottle',
      breastSide: [],
      milkType: ['breastmilk'],
      milkConsumed: 3,
      startTime: '2026-07-23T07:30:00',
      endTime: '2026-07-23T07:45:00',
      notes: 'Finished most of the bottle.'
    },
    {
      bottleSize: 5,
      feedingType: 'bottle',
      breastSide: [],
      milkType: ['formula'],
      milkConsumed: 4,
      startTime: '2026-07-23T10:15:00',
      endTime: '2026-07-23T10:35:00',
      notes: 'Fed calmly.'
    }
  ];

  const result = mapLogEntries({
    templateEntry: template,
    sourceEntries,
    trackerType: 'feeding'
  });

  assert.equal(result.length, 2);
  assert.equal(result[0].trackerType, 'feeding');
  assert.deepEqual(result[0].data, sourceEntries[0]);
  assert.equal(result[1].id, '2');
  assert.equal(result[1].data.notes, 'Fed calmly.');
});

test('supports other tracker types by using the provided trackerType', () => {
  const template = {
    id: '1',
    trackerType: 'sleep',
    createdAt: '2026-07-23T07:46:00',
    updatedAt: '2026-07-23T07:46:00',
    data: {
      startTime: '2026-07-23T07:30:00',
      endTime: '2026-07-23T07:45:00',
      location: 'crib',
      wakeReason: 'hungry',
      notes: 'Slept well.'
    }
  };

  const result = mapLogEntries({
    templateEntry: template,
    sourceEntries: [{
      startTime: '2026-07-23T07:30:00',
      endTime: '2026-07-23T07:45:00',
      location: 'crib',
      wakeReason: 'hungry',
      notes: 'Slept well.'
    }],
    trackerType: 'sleep'
  });

  assert.equal(result[0].trackerType, 'sleep');
  assert.deepEqual(result[0].data, {
    startTime: '2026-07-23T07:30:00',
    endTime: '2026-07-23T07:45:00',
    location: 'crib',
    wakeReason: 'hungry',
    notes: 'Slept well.'
  });
});
